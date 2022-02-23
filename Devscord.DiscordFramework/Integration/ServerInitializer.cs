using Autofac;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Integration.Services;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Discord.WebSocket;
using Serilog;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Framework
{
    public static class ServerInitializer
    {
        public static bool Initialized { get; private set; }

        public static void Initialize(DiscordSocketClient client, IComponentContext context)
        {
            if (Initialized)
            {
                return;
            }
            var sw = Stopwatch.StartNew();
            //todo ioc
            var discordClient = new DiscordClient(client, context);
            Server.Initialize(discordClient);
            Initialized = true;

            var waiting = 0;
            while (client.ConnectionState != Discord.ConnectionState.Connected)
            {
                Log.Information("Waiting for connection... {time}ms after initialization", waiting++ * 100);
                Task.Delay(100).Wait();
            }
            Log.Information("Bot is connected. {ms}ms", sw.ElapsedMilliseconds);
            sw.Stop();
        }

        //at now it is used only for tests
        public static void Initialize(IComponentContext context)
        {
            var discordClient = context.Resolve<IDiscordClient>();
            Server.Initialize(discordClient);

            Initialized = true;
        }
    }
}
