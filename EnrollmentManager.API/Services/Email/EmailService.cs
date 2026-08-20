using EnrollmentManager.API.Configurations;
using EnrollmentManager.API.Configurations;
using EnrollmentManager.API.Services.Interfaces;

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EnrollmentManager.API.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _settings;

    public EmailService(IOptions<EmailConfiguration> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string body)
    {
        MimeMessage message = new();

        message.From.Add(
            new MailboxAddress(
                _settings.From,
                _settings.From
            )
        );

        message.To.Add(
            MailboxAddress.Parse(to)
        );

        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = body
        };

        using SmtpClient smtp = new();

        await smtp.ConnectAsync(
            _settings.Host,
            _settings.Port,
            SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _settings.Username,
            _settings.Password
        );

        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);
    }
    
}