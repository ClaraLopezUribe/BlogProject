using System.ComponentModel.DataAnnotations;

namespace BlogProject.View_Models
{
    public class MailSettings
    {
        // Store SMTP settings; to configure and use SMTP server (i.e. google)
        [Required]
        public required string Mail { get; set; }  // TODO : change model property name to "Email"??? If I remember correctly this Mail conflicts with some other Mail property elsewhere
        public string? DisplayName { get; set; }
        public required string MailPassword { get; set; }
        public string MailHost { get; set; }
        public int MailPort { get; set; }
    }
}
