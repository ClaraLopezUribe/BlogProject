using BlogProject.View_Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BlogProject.Services
{
    public class EmailService : IBlogEmailSender
    {
        private readonly ILogger<EmailService> _logger;
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage)
        {
            try
            {

            
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                email.To.Add(MailboxAddress.Parse(_mailSettings.Mail));
                email.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = $"<b>{name}</b> has sent you an email and can be reached at: <b>{emailFrom}</b><br/><br/>{htmlMessage}";

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                // LEARN : As per ClaudeSonnet 3.5' recommendation, Add logging or console outut for debugging
                _logger.LogDebug("Connecting to SMTP server {Host} on port {Port}", _mailSettings.MailHost, _mailSettings.MailPort);

                await smtp.ConnectAsync(_mailSettings.MailHost, _mailSettings.MailPort, SecureSocketOptions.StartTls);

                _logger.LogDebug("Connected to successfully to SMTP server");

                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.MailPassword);

                _logger.LogDebug("Authenticated successfully");

                await smtp.SendAsync(email);
                _logger.LogDebug("Email sent successfully to {Recipient}", email.To);

                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log the full exception details
                _logger.LogError(ex, "Error sending email from {EmailFrom} with subject {Subject}", emailFrom, subject);

                throw;
            }
        }

        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.MailHost, _mailSettings.MailPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.MailPassword);

            await smtp.SendAsync(email);

            smtp.Disconnect(true);

            // Optional: Add code to allow attachments to be included

        }
    }
}
