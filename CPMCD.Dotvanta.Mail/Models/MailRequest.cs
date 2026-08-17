namespace CPMCD.Dotvanta.Mail.Models
{
    public class MailRequest
    {
        public string? To { get; set; }
        public string? Cc { get; set; }
        public string? Bcc { get; set; }
        public string Subject { get; set; } = string.Empty;

        /// <summary>Poora HTML (ya plain) body - fixed template package mein nahi hai,
        /// har app apna template khud bana kar bhejega.</summary>
        public string Body { get; set; } = string.Empty;

        public bool IsBodyHtml { get; set; } = true;

        public List<MailAttachmentData>? Attachments { get; set; }
    }

    /// <summary>
    /// Platform-agnostic attachment - IFormFile (ASP.NET Core specific) use nahi kiya,
    /// taaki ye package Web + Windows + Maui teeno se bhi call ho sake.
    /// </summary>
    public class MailAttachmentData
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
    }
}
