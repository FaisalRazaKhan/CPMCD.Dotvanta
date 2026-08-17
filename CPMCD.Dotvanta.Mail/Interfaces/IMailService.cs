using CPMCD.Dotvanta.Mail.Models;

namespace CPMCD.Dotvanta.Mail.Interfaces
{
    public interface IMailService
    {
        Task SendMailAsync(MailRequest request, CancellationToken cancellationToken = default);
    }
}
