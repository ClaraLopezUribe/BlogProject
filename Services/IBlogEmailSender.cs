using Microsoft.AspNetCore.Identity.UI.Services;
using RestSharp;

namespace BlogProject.Services
{
    public interface IBlogEmailSender : IEmailSender
    {
        Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage);

        //new Task<RestResponse> SendEmailAsync(string emailTo, string subject, string htmlMessage);
        
    }
}
