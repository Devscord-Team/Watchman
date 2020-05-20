using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Integration.Services;
using Discord.WebSocket;
using Serilog;

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
            var discordClient = new DiscordClient(client);
            Server.Initialize(discordClient);
            Log.Information("Server initialized");
            Initialized = true;
        }
    }
}
