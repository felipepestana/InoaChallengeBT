using System.Net.Mail;
using System.Text;
using System.Net;

namespace Inoa
{
    public enum EmailType 
    {
        None,
        Buy, 
        Sell
    } 

    public class MailService 
    {
        private MailConfig Config;
        private SmtpClient Client;

        public MailService(MailConfig mailConfig)
        {
            Config = mailConfig;
            Client = new SmtpClient(mailConfig.Host, mailConfig.Port)
            {
                Credentials = new NetworkCredential(mailConfig.FromEmail, mailConfig.Password),
                EnableSsl = true,
            };
        }

        public Task SendEmail(EmailType type, Stock asset, decimal curValue)
        {
            if(type == EmailType.None)
                return Task.CompletedTask;

            StringBuilder strBuilder = new();
            strBuilder.AppendLine("<h3>Inoa Stock Monitor</h3>");
            strBuilder.AppendLine();
            strBuilder.AppendLine("<p>Hello,</p>");
            strBuilder.AppendLine($"<p>The current price for the monitored asset {asset.Name} is {curValue:C}</p>");
            strBuilder.AppendLine($"<p>Our recommendation is to <b>{type}</b> stocks</p>");
            strBuilder.AppendLine();
            strBuilder.AppendLine("<p>All the best,</p>");

            var mailMessage = new MailMessage
            {
                From = new MailAddress(Config.FromEmail),
                Subject = $"Inoa Stock Monitor - {asset.Name} {type}",
                Body = strBuilder.ToString(),
                IsBodyHtml = true,
            };
            mailMessage.To.Add(Config.ToEmail);

            foreach(string ccEmail in Config.CcEmail)
            {
                mailMessage.CC.Add(ccEmail);
            }
            
           return Client.SendMailAsync(mailMessage);
        }
    }
}