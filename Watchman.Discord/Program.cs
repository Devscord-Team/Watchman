
using Newtonsoft.Json;
using System.IO;

namespace Watchman.Discord
{
    
    class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            var configPath = "config.json";
#if RELEASE
            configPath = "config-prod.json";
#endif
            var configuration = JsonConvert.DeserializeObject<DiscordConfiguration>(File.ReadAllText(configPath));
            new WatchmanBot(configuration).Start().GetAwaiter().GetResult();
        }
    }
}
