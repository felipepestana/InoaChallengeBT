using Microsoft.Extensions.Configuration;

namespace Inoa
{
    class Program
    {
        private static MonitorService? Monitor;
        private static int SleepTime;
        private static int MaxErrorsToAbort;

        static async Task Main(string[] args)
        {
            try
            {
                LoadConfig();
                if(Monitor is null)
                    return;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception found when trying to load configuration from envs: {ex.Message}");
                return;
            }

            Stock? asset = LoadArgs(args);
            if(asset == null)
                return;

            Console.WriteLine($"Begin monitoring asset {asset.Name}");

            int countErrors = 0;
            do {
                bool result = await Monitor.MonitorAsset(asset);
                if(!result && MaxErrorsToAbort > 0)
                {
                    countErrors++;
                    if(countErrors >= MaxErrorsToAbort)
                    {
                        Console.WriteLine($"Aborting executions after max number of errors {MaxErrorsToAbort}");
                        return;
                    }
                }

                await Task.Delay(SleepTime * 1000);
            } while(true);
        }

        private static void LoadConfig()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            SleepTime = config.GetValue<int>("SleepTime");
            if(SleepTime <= 0)
                SleepTime = 60;

            MaxErrorsToAbort = config.GetValue<int>("MaxErrorsToAbort");

            MailConfig mailConfig = new MailConfig(config.GetSection("MailConfig"));
            MailService mailService = new MailService(mailConfig);

            QueryService queryService = new QueryService();
            Monitor = new MonitorService(queryService, mailService);
        }

        private static Stock? LoadArgs(string[] args)
        {
            if(args.Length < 3)
            {
                Console.WriteLine("Missing expected parameters on program initialization");
                return null;
            }

            Stock? asset = null;
            try
            {
                asset = new Stock(args[0], args[1], args[2]);
            } 
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return asset;
        }
    }
}
