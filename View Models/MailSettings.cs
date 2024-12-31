namespace BlogProject.View_Models
{
    public class MailSettings
    {
        // Store SMTP settings; to configure and use SMTP server (i.e. google)
        public string? Mail { get; set; }
        public string? DisplayName { get; set; }
        public string? MailPassword { get; set; }
        public string? MailHost { get; set; }
        public int MailPort { get; set; }
    }
}
