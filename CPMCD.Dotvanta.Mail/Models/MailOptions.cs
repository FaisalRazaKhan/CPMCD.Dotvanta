namespace CPMCD.Dotvanta.Mail.Models
{
    /// <summary>
    /// Consuming app apni appsettings.json ("MailSettings" section) se ya code se
    /// isko bind kar ke DI mein register karega - koi bhi hardcoded credential
    /// package ke andar nahi hai.
    /// </summary>
    public class MailOptions
    {
        public const string SectionName = "MailSettings";

        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 25;
        public string SenderEmail { get; set; } = string.Empty;

        /// <summary>Blank rakho agar UseDefaultCredentials = true (Windows/domain auth).</summary>
        public string? SenderPassword { get; set; }

        public bool UseDefaultCredentials { get; set; } = true;
        public bool EnableSsl { get; set; } = false;

        /// <summary>Koi "To" na diya jaye to fallback recipient.</summary>
        public string? DefaultTo { get; set; }
    }
}
