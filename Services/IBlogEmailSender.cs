using Microsoft.AspNetCore.Identity.UI.Services;

namespace BlogProject.Services
{
    public interface IBlogEmailSender : IEmailSender
    {
        Task SendContactEmailAsync(string emailForm, string name, string subject, string htmlMessage);
    }
}
