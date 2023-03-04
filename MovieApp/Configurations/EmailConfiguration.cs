namespace GOSBackend.Configurations
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string SmtpServer { get; set; }  =string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }  = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public bool ShouldSendEmail { get; set; }

        public string PopServer { get; set; } = string.Empty;
        public int PopPort { get; set; }
        public string PopUsername { get; set; } = string.Empty;
        public string PopPassword { get; set; } = string.Empty;
    }
    public interface IEmailConfiguration
    {
        string SmtpServer { get; }
        int SmtpPort { get; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }
        bool ShouldSendEmail { get; set; }

        string PopServer { get; }
        int PopPort { get; }
        string PopUsername { get; }
        string PopPassword { get; }
    }
}
