using Microsoft.Extensions.Configuration;

namespace Inoa
{
    public class MailConfig
    {
        public string Host {get; private set; }
        public int Port {get; private set; }

        public string FromEmail {get; private set; }
        public string Password {get; private set; }

        public string ToEmail {get; private set; }
        public List<string> CcEmail {get; private set; }

        public MailConfig(IConfiguration configuration)
        {
            Host = configuration.GetValue<string>("Host") ?? throw new ArgumentNullException("Empty email host parameter");
            Port = configuration.GetValue<int>("Port");

            FromEmail = configuration.GetValue<string>("FromEmail") ?? throw new ArgumentNullException("Empty from email parameter");
            Password = configuration.GetValue<string>("Password") ?? throw new ArgumentNullException("Empty email password parameter");

            ToEmail = configuration.GetValue<string>("ToEmail") ?? throw new ArgumentNullException("Empty to email parameter");
            CcEmail = configuration.GetSection("CcEmail").Get<List<string>>() ?? new List<string>();
        }
    }
}