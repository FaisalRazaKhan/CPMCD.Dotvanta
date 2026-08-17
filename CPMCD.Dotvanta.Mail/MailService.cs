using System.Net;
using System.Net.Mail;
using System.Text;
using CPMCD.Dotvanta.Mail.Interfaces;
using CPMCD.Dotvanta.Mail.Models;
using Microsoft.Extensions.Options;

namespace CPMCD.Dotvanta.Mail
{
    public class MailService : IMailService
    {
        private readonly MailOptions _options;

        public MailService(IOptions<MailOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task SendMailAsync(MailRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(_options.SmtpHost))
                throw new InvalidOperationException("MailOptions.SmtpHost configure nahi hua.");
            if (string.IsNullOrWhiteSpace(_options.SenderEmail))
                throw new InvalidOperationException("MailOptions.SenderEmail configure nahi hua.");

            using var mail = new MailMessage
            {
                From = new MailAddress(_options.SenderEmail),
                Subject = request.Subject,
                Body = request.Body,
                IsBodyHtml = request.IsBodyHtml,
                BodyEncoding = Encoding.UTF8,
                Priority = MailPriority.Normal
            };

            AddAddresses(mail.To, string.IsNullOrWhiteSpace(request.To) ? _options.DefaultTo : request.To);
            AddAddresses(mail.CC, request.Cc);
            AddAddresses(mail.Bcc, request.Bcc);

            if (mail.To.Count == 0)
                throw new InvalidOperationException("Koi 'To' recipient nahi mila (na request mein, na MailOptions.DefaultTo mein).");

            var attachmentStreams = new List<MemoryStream>();
            if (request.Attachments != null)
            {
                foreach (var file in request.Attachments)
                {
                    var stream = new MemoryStream(file.Content);
                    attachmentStreams.Add(stream);
                    mail.Attachments.Add(new Attachment(stream, file.FileName, file.ContentType));
                }
            }

            try
            {
                using var smtp = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
                {
                    EnableSsl = _options.EnableSsl,
                    UseDefaultCredentials = _options.UseDefaultCredentials
                };

                if (!_options.UseDefaultCredentials && !string.IsNullOrWhiteSpace(_options.SenderPassword))
                {
                    smtp.Credentials = new NetworkCredential(_options.SenderEmail, _options.SenderPassword);
                }

                await smtp.SendMailAsync(mail, cancellationToken);
            }
            finally
            {
                foreach (var s in attachmentStreams) s.Dispose();
            }
        }

        private static void AddAddresses(MailAddressCollection list, string? addresses)
        {
            if (string.IsNullOrWhiteSpace(addresses)) return;
            foreach (var addr in addresses.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                list.Add(addr.Trim());
        }
    }
}
