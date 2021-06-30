namespace GamingStore.Services.Email
{
    public class AuthMessageOptions
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPortNumber { get; set; } = 587;
        public string AppName { get; set; } = "The Gaming Store";
        public string GmailUser { get; set; }
        public string GmailKey { get; set; }
    }
}