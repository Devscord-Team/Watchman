using System;
using Newtonsoft.Json;
using Serilog;
using System.IO;
using System.Threading.Tasks;

namespace Watchman.Discord
{
    class Program
    {
        const int HOW_MANY_EXCEPTIONS_IN_SHORT_TIME_TO_STOP_BOT = 3;

        public static async Task Main(string[] args)
        {
#if DEBUG
            var configPath = "config.json";
#elif RELEASE
            var configPath = "config-prod.json";
#endif
            var configuration = JsonConvert.DeserializeObject<DiscordConfiguration>(File.ReadAllText(configPath));
            
            var watchman = new WatchmanBot(configuration);
            var workflowBuilder = watchman.GetWorkflowBuilder();

            var lastException = DateTime.MinValue;
            var numberOfExceptionsInLastTime = 0;

            while (true)
            {
                try
                {
                    await workflowBuilder.Run();
                }
                catch (Exception e)
                {
                    Log.Error(e, "Not handled exception!");
                    if (numberOfExceptionsInLastTime++ > HOW_MANY_EXCEPTIONS_IN_SHORT_TIME_TO_STOP_BOT)
                    {
                        break;
                    }
                    if (lastException < DateTime.Now.AddMinutes(-30))
                    {
                        numberOfExceptionsInLastTime = 0;
                    }
                }
            }
        }
    }
}
