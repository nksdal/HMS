using System.Net;
using System.Net.Mail;
using HMS.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HMS.Infrastructure.Email;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<EmailSettings> settings, ILogger<EmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailConfirmationAsync(string toEmail, string confirmationToken)
    {
        var confirmationLink =
            $"{_settings.AppBaseUrl}/confirm-email.html" +
            $"?email={Uri.EscapeDataString(toEmail)}" +
            $"&token={Uri.EscapeDataString(confirmationToken)}";

        var body = $@"
<div style=""font-family:Arial,sans-serif;max-width:480px"">
  <h2>Welcome to HMS</h2>
  <p>Click the button below to activate your account:</p>
  <p>
    <a href=""{confirmationLink}""
       style=""background:#2563eb;color:#fff;padding:12px 24px;border-radius:6px;text-decoration:none;display:inline-block"">
      Verify My Account
    </a>
  </p>
  <p>Or paste this link into your browser:</p>
  <p style=""word-break:break-all;font-size:12px;color:#555"">{confirmationLink}</p>
  <p>This link expires in 24 hours.</p>
</div>";

        using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.UseSsl
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_settings.Sender, "HMS"),
            Subject = "Confirm your HMS account",
            Body = body,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        _logger.LogInformation("Sending confirmation email via {Server}:{Port} to {Recipient}",
            _settings.SmtpServer, _settings.Port, toEmail);

        await client.SendMailAsync(message);

        _logger.LogInformation("Confirmation email sent to {Recipient}", toEmail);
    }
}
