namespace OpenRIMS.PV.Main.API.Infrastructure.Settings
{
    public class SMTPSettings
    {
        public bool Enabled { get; set; }
        public string SmtpHost { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public string MailboxUserName { get; set; }
        public string MailboxPassword { get; set; }
        public string MailboxAddress { get; set; }
    }
}
