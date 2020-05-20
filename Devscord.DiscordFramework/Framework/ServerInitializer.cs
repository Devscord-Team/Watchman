using Discord.WebSocket;

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
            Server.Initialize(client);
            Initialized = true;
        }
    }
}
