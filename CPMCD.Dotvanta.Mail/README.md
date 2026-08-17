# CPMCD.Dotvanta.Mail

Reusable SMTP email service for .NET applications.

`CPMCD.Dotvanta.Mail` provides a DI-friendly abstraction for sending email without hardcoding SMTP credentials or application-specific HTML templates inside the library.

## Features

- SMTP email sending
- Dependency Injection support
- `appsettings.json` configuration
- Code-based configuration
- Plain text or HTML email
- To / CC / BCC
- Multiple recipients
- Attachments
- Platform-independent attachment model
- No hardcoded SMTP credentials
- No fixed application-specific email template

## Installation

```bash
dotnet add package CPMCD.Dotvanta.Mail
```

## Target Framework

```text
net9.0
```

## Configuration

Add a `MailSettings` section to the consuming application's configuration:

```json
{
  "MailSettings": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 25,
    "SenderEmail": "noreply@example.com",
    "UseDefaultCredentials": true,
    "EnableSsl": false,
    "DefaultTo": "fallback@example.com"
  }
}
```

### Password Security

Do not store SMTP passwords directly in source-controlled `appsettings.json`.

For development, use .NET User Secrets:

```bash
dotnet user-secrets set "MailSettings:SenderPassword" "your-password"
```

For production, prefer:

- Environment variables
- Azure Key Vault
- AWS Secrets Manager
- Another approved secrets manager

If username/password authentication is required:

```json
{
  "MailSettings": {
    "UseDefaultCredentials": false,
    "EnableSsl": true
  }
}
```

Then provide `MailSettings:SenderPassword` securely.

## ASP.NET Core Registration

```csharp
using CPMCD.Dotvanta.Mail;

builder.Services.AddCpmcdMailService(builder.Configuration);
```

The extension reads:

```text
MailSettings
```

from the application's configuration.

## Code-Based Configuration

Configuration can also be provided directly:

```csharp
builder.Services.AddCpmcdMailService(options =>
{
    options.SmtpHost = "smtp.example.com";
    options.SmtpPort = 587;
    options.SenderEmail = "noreply@example.com";
    options.UseDefaultCredentials = false;
    options.EnableSsl = true;
});
```

## Send Email

Inject `IMailService`:

```csharp
using CPMCD.Dotvanta.Mail.Interfaces;
using CPMCD.Dotvanta.Mail.Models;

public class MailController : ControllerBase
{
    private readonly IMailService _mailService;

    public MailController(IMailService mailService)
    {
        _mailService = mailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send()
    {
        var request = new MailRequest
        {
            To = "user@example.com",
            Subject = "Welcome",
            Body = "<h1>Welcome to Dotvanta</h1>",
            IsBodyHtml = true
        };

        await _mailService.SendMailAsync(request);

        return Ok();
    }
}
```

## CC / BCC

```csharp
var request = new MailRequest
{
    To = "user@example.com",
    Cc = "manager@example.com",
    Bcc = "audit@example.com",
    Subject = "Application Update",
    Body = "Your application has been updated.",
    IsBodyHtml = false
};
```

Multiple recipients can be separated by comma or semicolon:

```text
user1@example.com; user2@example.com
```

## Default Recipient

If `MailRequest.To` is empty, the service can use:

```json
"DefaultTo": "fallback@example.com"
```

This is useful for controlled internal environments.

## Attachments

Attachments use the platform-independent `MailAttachmentData` model.

```csharp
var request = new MailRequest
{
    To = "user@example.com",
    Subject = "Report",
    Body = "<p>Please find the report attached.</p>",
    IsBodyHtml = true,
    Attachments = new List<MailAttachmentData>
    {
        new MailAttachmentData
        {
            FileName = "report.pdf",
            Content = pdfBytes,
            ContentType = "application/pdf"
        }
    }
};

await _mailService.SendMailAsync(request);
```

## ASP.NET Core `IFormFile` Example

Convert uploaded files into the package's platform-independent model:

```csharp
var attachments = new List<MailAttachmentData>();

foreach (var file in files)
{
    using var stream = new MemoryStream();

    await file.CopyToAsync(stream);

    attachments.Add(new MailAttachmentData
    {
        FileName = file.FileName,
        Content = stream.ToArray(),
        ContentType = file.ContentType
    });
}
```

Then:

```csharp
request.Attachments = attachments;
```

## Models

### MailOptions

Controls SMTP configuration:

```csharp
MailOptions.SmtpHost
MailOptions.SmtpPort
MailOptions.SenderEmail
MailOptions.SenderPassword
MailOptions.UseDefaultCredentials
MailOptions.EnableSsl
MailOptions.DefaultTo
```

### MailRequest

Controls individual email content:

```csharp
MailRequest.To
MailRequest.Cc
MailRequest.Bcc
MailRequest.Subject
MailRequest.Body
MailRequest.IsBodyHtml
MailRequest.Attachments
```

### MailAttachmentData

Platform-independent attachment:

```csharp
MailAttachmentData.FileName
MailAttachmentData.Content
MailAttachmentData.ContentType
```

## Why Templates Are Not Hardcoded

The library intentionally does not contain a fixed email template such as:

```text
Dear User...
```

The consuming application owns:

- Branding
- HTML templates
- Localization
- Email subject
- Business content
- Recipient logic

This keeps the package reusable across applications.

## Security Recommendations

- Never commit SMTP passwords.
- Prefer a secrets manager in production.
- Use TLS/SSL when supported by the SMTP server.
- Validate recipient addresses in the consuming application.
- Restrict attachment size and file types at the application boundary.
- Do not put sensitive data into logs.
- Avoid exposing SMTP configuration through public APIs.

## Build

```bash
dotnet restore
dotnet build -c Release
dotnet pack -c Release
```

## NuGet README Metadata

For NuGet to display this README on the package page, add:

```xml
<PackageReadmeFile>README.md</PackageReadmeFile>
```

and:

```xml
<ItemGroup>
  <None Include="README.md"
        Pack="true"
        PackagePath="\" />
</ItemGroup>
```

## Dependencies

The project currently uses:

```text
Microsoft.Extensions.Options.ConfigurationExtensions
Microsoft.Extensions.DependencyInjection.Abstractions
```

## Author

**CPMCD : Faisal Raza Khan**

## License

Add the final project license before public distribution.
