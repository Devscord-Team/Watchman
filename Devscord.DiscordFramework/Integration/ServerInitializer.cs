using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Integration.Services;
using Discord.WebSocket;
using Serilog;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Framework
{
    public static class ServerInitializer
    {
        public static bool Initialized { get; private set; }

        public static void Initialize(DiscordSocketClient client)
        {
            if (Initialized)
            {
                return;
            }
            var sw = Stopwatch.StartNew();
            var discordClient = new DiscordClient(client);
            Server.Initialize(discordClient);
            Initialized = true;

            int waiting = 0;
            while (client.ConnectionState != Discord.ConnectionState.Connected)
            {
                Log.Information("Waiting for connection... {time}ms after initialization", waiting++ * 100);
                Task.Delay(100).Wait();
            }
            Log.Information("Bot is connected. {ticks}ticks | {ms}ms", sw.ElapsedTicks, sw.ElapsedMilliseconds);
            sw.Stop();
        }
    }
}
