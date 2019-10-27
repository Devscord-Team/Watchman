using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Discord.Framework
{
    public static class Server
    {
        private static DiscordSocketClient _client;

        public static bool Initialized = false;

        public static void Initialize(DiscordSocketClient client)
        {
            if (Initialized)
            {
                return;
            }
            _client = client;
            Initialized = true;
        }

        //todo optimalize it by using field and delegates like _client.RoleCreated etc
        public static IReadOnlyCollection<SocketRole> GetRoles(ulong guildId)
        {
            return _client.GetGuild(guildId).Roles;
        }
    }
}
