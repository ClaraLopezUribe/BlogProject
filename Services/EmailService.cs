using BlogProject.View_Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using RestSharp;
using RestSharp.Authenticators;

namespace BlogProject.Services
{
    public class EmailService : IBlogEmailSender
    {
        private readonly MailSettings _mailSettings;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public EmailService(IOptions<MailSettings> mailSettings, IConfiguration configuration, IEmailSender emailSender)
        {
            _mailSettings = mailSettings.Value;
            _configuration = configuration;
            _emailSender = emailSender;
        }
        public async Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage)
        {
            var myEmail = _mailSettings.Mail ?? Environment.GetEnvironmentVariable("Mail");
            if (string.IsNullOrWhiteSpace(emailFrom) || !MailboxAddress.TryParse(emailFrom, out var mailbox))
            {
                throw new ArgumentException("A valid From email is required.", nameof(emailFrom));
            }
            else
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailFrom));
                email.To.Add(MailboxAddress.Parse(myEmail));
                email.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = $"<b>{name}</b> has sent you an email and can be reached at: <b>{emailFrom}</b><br/><br/>{htmlMessage}";

                email.Body = builder.ToMessageBody();

                // Log into smtp client
                using SmtpClient smtpClient = new();

                try
                {
                    var host = _mailSettings.MailHost ?? Environment.GetEnvironmentVariable("MailHost");
                    int port;
                    if (_mailSettings.MailPort != 0)
                    {
                        port = _mailSettings.MailPort;
                    }
                    else
                    {
                        port = int.Parse(Environment.GetEnvironmentVariable("MailPort")!);
                    }

                    var password = _mailSettings.MailPassword ?? Environment.GetEnvironmentVariable("MailPassword");

                    await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(myEmail, password);

                    await smtpClient.SendAsync(email);
                    await smtpClient.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                    throw;
                }
            }
        }

        //***** Mailgun Email API***** //
        public async Task<RestResponse> SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {

            /*** Validate the email address ***/
            if (string.IsNullOrWhiteSpace(emailTo))
            {
                throw new ArgumentException("A valid email is required.", nameof(emailTo));
            }

            //Get the API value from environment variable configuration
            var emailAPI = _configuration.GetSection("MailgunSettings")["EMAIL_API"] ?? Environment.GetEnvironmentVariable("EMAIL_API");

            if (string.IsNullOrWhiteSpace(emailAPI))
            {
                throw new ArgumentException("A valid email API is required.", nameof(emailAPI));
            }

            var options = new RestClientOptions("https://api.mailgun.net")
            { Authenticator = new HttpBasicAuthenticator("api", emailAPI) };

            var client = new RestClient(options);
            var request = new RestRequest("/v3/codelifeuploaded.com/messages", Method.Post);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("from", "<admin@codelifeuploaded.com>");
            request.AddParameter("to", emailTo);
            request.AddParameter("subject", subject);
            request.AddParameter("html", htmlMessage);


            return await client.ExecuteAsync(request);
        }

        Task IEmailSender.SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return SendEmailAsync(email, subject, htmlMessage);
        }
    }
}





//        // ***** WORKING COPY ***** //
//        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
//        {
//            /*** Validate the email address ***/
//            if (string.IsNullOrWhiteSpace(emailTo) || !MailboxAddress.TryParse(emailTo, out var mailbox))
//            {
//                throw new ArgumentException("A valid email is required.", nameof(emailTo));
//            }

//            /*** Load SMTP settings from environment variables ***/
//            var emailSender = _mailSettings.Mail ?? Environment.GetEnvironmentVariable("Mail");
//            var host = _mailSettings.MailHost ?? Environment.GetEnvironmentVariable("MailHost");
//            int port;
//            if (_mailSettings.MailPort != 0)
//            {
//                port = _mailSettings.MailPort;
//            }
//            else
//            {
//                port = int.Parse(Environment.GetEnvironmentVariable("MailPort"));
//            }

//            var password = _mailSettings.MailPassword ?? Environment.GetEnvironmentVariable("MailPassword");

//            //***Create the email ***
//            MimeMessage newEmail = new();
//            newEmail.From.Add(MailboxAddress.Parse(emailSender));
//            newEmail.To.Add(MailboxAddress.Parse(emailTo));
//            newEmail.Subject = subject;

//            BodyBuilder emailBody = new();
//            emailBody.HtmlBody = htmlMessage;
//            newEmail.Body = emailBody.ToMessageBody();

//            /*** Log into smtp client ***/
//            using SmtpClient smtp = new();

//            try
//            {
//                /*** Connect to SMTP server ***/
//                smtp.Connect(host, port, SecureSocketOptions.StartTls);

//                /*** Authenticate ***/
//                smtp.Authenticate(emailSender, password);

//                /*** Send the email ***/
//                await smtp.SendAsync(newEmail);
//            }
//            /***CATCH and log any errors ***/
//            catch (TimeoutException ex)
//            {
//                //LOG TIMEOUT EXCEPTIONS SPECIFICALLY
//                Console.Error.WriteLine($"Email send timed out: {ex.Message}");
//                return;
//            }
//            catch (Exception ex)
//            {
//                //LOG ANY OTHER ERRORS
//                Console.Error.WriteLine($"Error sending email: {ex.Message}");
//                return;

//            }
//            //*** FINALLY, DISCONNECT ***
//            finally
//            {
//                if (smtp.IsConnected)
//                {
//                    await smtp.DisconnectAsync(true);
//                }
//            }

//            // Optional: Add code to allow attachments to be included
//        }
//    }
//}


//        //// **** Refactored to mirror ContactPro EmailService **** ////
//        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
//        {
//            // Use null coalescing operator to instruct email sender to get the local value, but if null will get the right hand side value from Railway
//            var emailSender = _mailSettings.Mail ?? Environment.GetEnvironmentVariable("Mail");

//            MimeMessage newEmail = new();

//            // BLOG : Why use .Sender instead of .From.Add ???? In ContactPRO one use could send emails to other users...so perhaps this is more appropriate for that scenario??? In BlogProject, all emails will come from the same email address, so .From.Add might be more appropriate???
//            //newEmail.Sender = MailboxAddress.Parse(emailSender);
//            newEmail.From.Add(MailboxAddress.Parse(emailSender));

//            // BLOG : in ContactPro a for loop is used here because an email could be sent to a group consisting of multiple emails. Is this a feature that could be useful in the BlogProject???
//            //foreach (var emailAddress in email.Split(";"))
//            //{
//            //    newEmail.To.Add(MailboxAddress.Parse(emailAddress));
//            //}

//            newEmail.To.Add(MailboxAddress.Parse(email));
//            newEmail.Subject = subject;

//            BodyBuilder emailBody = new();
//            emailBody.HtmlBody = htmlMessage;

//            newEmail.Body = emailBody.ToMessageBody();

//            // Log into smtp client
//            using SmtpClient smtpClient = new();

//            try
//            {
//                var host = _mailSettings.MailHost ?? Environment.GetEnvironmentVariable("MailHost");
//                var port = _mailSettings.MailPort != 0 ? _mailSettings.MailPort : int.Parse(Environment.GetEnvironmentVariable("MailPort"));
//                var password = _mailSettings.MailPassword ?? Environment.GetEnvironmentVariable("MailPassword");

//                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
//                await smtpClient.AuthenticateAsync(emailSender, password);

//                await smtpClient.SendAsync(newEmail);
//                await smtpClient.DisconnectAsync(true);
//            }
//            catch (Exception ex)
//            {
//                var error = ex.Message;
//                throw;
//            }
//        }
//    }
//}




//        // **** ORIGINAL MAIN BRANCH VERSION BELOW: *******
//        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
//        {
//            var email = new MimeMessage();
//            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
//            email.To.Add(MailboxAddress.Parse(emailTo));
//            email.Subject = subject;

//            var builder = new BodyBuilder();
//            builder.HtmlBody = htmlMessage;

//            email.Body = builder.ToMessageBody();

//            using var smtp = new SmtpClient();
//            smtp.Connect(_mailSettings.MailHost, _mailSettings.MailPort, SecureSocketOptions.StartTls);
//            smtp.Authenticate(_mailSettings.Mail, _mailSettings.MailPassword);

//            await smtp.SendAsync(email);

//            smtp.Disconnect(true);

//            // Optional: Add code to allow attachments to be included

//        }
//    }
//}

//        // ***** COPILOT SUGGESTIONS *****
//        public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string htmlMessage)
//        {
//            // Load SMTP settings from environment variables
//            string smtpHost = Environment.GetEnvironmentVariable("MAIL_HOST");
//            int smtpPort = int.Parse(Environment.GetEnvironmentVariable("MAIL_PORT") ?? "587");
//            string smtpUser = Environment.GetEnvironmentVariable("MAIL_USER");
//            string smtpPass = Environment.GetEnvironmentVariable("MAIL_PASSWORD");

//            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();

//            try
//            {
//                // Connect to SMTP server
//                await smtpClient.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

//                // Authenticate
//                await smtpClient.AuthenticateAsync(smtpUser, smtpPass);

//                // Create the email
//                var email = new MimeKit.MimeMessage();
//                email.From.Add(new MimeKit.MailboxAddress("My Blog", smtpUser));
//                email.To.Add(new MimeKit.MailboxAddress("", recipientEmail));
//                email.Subject = subject;
//                email.Body = new MimeKit.TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

//                // Send the email
//                await smtpClient.SendAsync(email);

//                return true; // Success
//            }
//            catch (System.TimeoutException ex)
//            {
//                // Log timeout specifically
//                Console.Error.WriteLine($"Email send timed out: {ex.Message}");
//                return false;
//            }
//            catch (Exception ex)
//            {
//                // Log any other errors
//                Console.Error.WriteLine($"Error sending email: {ex.Message}");
//                return false;
//            }
//            finally
//            {
//                // Always disconnect
//                if (smtpClient.IsConnected)
//                    await smtpClient.DisconnectAsync(true);
//            }
//        }
//    }
//}
