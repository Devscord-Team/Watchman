using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

namespace Watchman.Discord
{
    [ExcludeFromCodeCoverage]
    internal class Program
    {
        public static void Main(string[] args)
        {
            //old version, not supported

            /*
#if DEBUG
            var configPath = "config.json";
#elif RELEASE
            var configPath = "config-prod.json";
#endif
            var configuration = JsonConvert.DeserializeObject<DiscordConfiguration>(File.ReadAllText(configPath));

            var watchman = new WatchmanBot(configuration);
            var workflowBuilder = watchman.GetWorkflowBuilder();
            workflowBuilder.Build();
            Thread.Sleep(Timeout.Infinite);
            */
        }
    }
}