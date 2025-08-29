using Microsoft.AspNetCore.Identity.UI.Services;

namespace BlogProject.Services
{
    public interface IBlogEmailSender : IEmailSender
    {
        Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage);


        // TODO : ADD this to test if it help resolve the error on post of mimemessage related actions. Tested, and did not resolve error
        //new Task SendEmailAsync(string emailTo, string subject, string htmlMessage);
    }
}
