using BlogProject.View_Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BlogProject.Services
{
    public class EmailService : IBlogEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage)
        {
            var myEmail = _mailSettings.Mail ?? Environment.GetEnvironmentVariable("Mail");

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailFrom));
            email.To.Add(MailboxAddress.Parse(myEmail));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = $"<b>{name}</b> has sent you an email and can be reached at: <b>{emailFrom}</b><br/><br/>{htmlMessage}";

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.MailHost, _mailSettings.MailPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.MailPassword);

            await smtp.SendAsync(email);

            smtp.Disconnect(true);

        }
        //// **** Refactored to mirror ContactPro EmailService **** ////
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Use null coalescing operator to instruct email sender to get the local value, but if null will get the right hand side value // LEARN : Why check local value first and not the Environment value???
            var emailSender = _mailSettings.Mail ?? Environment.GetEnvironmentVariable("Mail");

            MimeMessage newEmail = new();

            // LEARN : Why use .Sender instead of .From ???? In ContactPRO one use could send emails to other users...so perhaps this is more appropriate for that scenario??? In BlogProject, all emails will come from the same email address. so .From might be more appropriate???
            //newEmail.Sender = MailboxAddress.Parse(emailSender);
            newEmail.From.Add(MailboxAddress.Parse(emailSender));

            // LEARN : was this loop in ContactPro because an email could be sent to a group consisting of multiple emails? is this a feature that could be needed in the BlogProject? If not delete.
            //foreach (var emailAddress in email.Split(";"))
            //{
            //    newEmail.To.Add(MailboxAddress.Parse(emailAddress));
            //}

            newEmail.To.Add(MailboxAddress.Parse(email));
            newEmail.Subject = subject;

            BodyBuilder emailBody = new();
            emailBody.HtmlBody = htmlMessage;

            newEmail.Body = emailBody.ToMessageBody();

            // Log into smtp client
            using SmtpClient smtpClient = new();

            try
            {
                var host = _mailSettings.MailHost ?? Environment.GetEnvironmentVariable("MailHost");
                var port = _mailSettings.MailPort != 0 ? _mailSettings.MailPort : int.Parse(Environment.GetEnvironmentVariable("MailPort"));
                var password = _mailSettings.MailPassword ?? Environment.GetEnvironmentVariable("MailPassword");

                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender, password);

                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }



        //public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        //{
        //    var newEmail = new MimeMessage();
        //    newEmail.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
        //    newEmail.To.Add(MailboxAddress.Parse(email));
        //    newEmail.Subject = subject;

        //    var builder = new BodyBuilder();
        //    builder.HtmlBody = htmlMessage;

        //    newEmail.Body = builder.ToMessageBody();

        //    using var smtp = new SmtpClient();
        //    smtp.Connect(_mailSettings.MailHost, _mailSettings.MailPort, SecureSocketOptions.StartTls);
        //    smtp.Authenticate(_mailSettings.Mail, _mailSettings.MailPassword);

        //    await smtp.SendAsync(newEmail);

        //    smtp.Disconnect(true);

        //    // Optional: Add code to allow attachments to be included

        //}
    }
}
