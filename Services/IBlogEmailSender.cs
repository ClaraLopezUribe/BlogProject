using Microsoft.AspNetCore.Identity.UI.Services;

namespace BlogProject.Services
{
    public interface IBlogEmailSender : IEmailSender
    {
        Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage);

        new Task SendEmailAsync(string emailTo, string subject, string htmlMessage);
    }
}
