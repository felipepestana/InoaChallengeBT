namespace Inoa
{
    public class MonitorService 
    {
        private QueryService Querier;
        private MailService Mailer;

        public MonitorService(QueryService queryService, MailService mailService)
        {
            Querier = queryService;
            Mailer = mailService;
        }

        public async Task<bool> MonitorAsset(Stock asset)
        {
            decimal currentValue;
            try
            {
                currentValue = await Querier.QueryAsset(asset);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception found when trying to query asset information: {asset.Name} : {ex.Message}");
                return false;
            }
            
            Console.WriteLine($"Current value for asset {asset.Name} is {currentValue:C}");

            EmailType email = EmailType.None;
            if(asset.isLesser(currentValue))
                email = EmailType.Buy;
            else if(asset.isGreater(currentValue))
                email = EmailType.Sell;

            try
            {
                await Mailer.SendEmail(email, asset, currentValue);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception found when trying to send email: {ex.Message}");
                return false;
            }

            if(email != EmailType.None)
                Console.WriteLine($"Sent email to {email} asset {asset.Name}");

            return true;
        }
    }
}