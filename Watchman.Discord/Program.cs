using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Watchman.Discord.Framework;

namespace Watchman.Discord
{
    
    class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            //option for tests, but remember -> token should NEVER be in repository, even as a single commit
            //safer option is to create config.json file and put token there

            //var configuration = new DiscordConfiguration
            //{
            //    Token = "XXX"
            //};
            //var watchman = new WatchmanBot(configuration);

            var watchman = new WatchmanBot();
#else
            var watchman = new WatchmanBot();
#endif

            watchman.Start()
                .GetAwaiter()
                .GetResult();
        }
    }
}
