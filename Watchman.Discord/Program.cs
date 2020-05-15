using System;
using Newtonsoft.Json;
using Serilog;
using System.IO;
using System.Threading.Tasks;

namespace Watchman.Discord
{
    class Program
    {
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
            while (true)
            {
                try
                {
                    await workflowBuilder.Run();
                }
                catch (Exception e)
                {
                    Log.Error($"Not handled exception: {e.Message} stack trace: {e.StackTrace}");
                }
            }
        }
    }
}
