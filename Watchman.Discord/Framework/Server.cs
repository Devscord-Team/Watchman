using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Discord.Framework
{
    public static class Server
    {
        private static DiscordSocketClient _client;
        private static IMongoDatabase _database;

        public static bool Initialized = false;

        public static void Initialize(DiscordSocketClient client, string mongoDbConnectionString)
        {
            if (Initialized)
            {
                return;
            }
            _client = client;
            _database = new MongoClient(mongoDbConnectionString).GetDatabase("devscord");
            Initialized = true;
        }

        public static SocketChannel GetChannel(ulong channelId)
        {
            return _client.GetChannel(channelId);
        }

        //todo optimalize it by using field and delegates like _client.RoleCreated etc
        public static IReadOnlyCollection<SocketRole> GetRoles(ulong guildId)
        {
            return _client.GetGuild(guildId).Roles;
        }

        public static SocketUser GetUser(ulong userId)
        {
            return _client.GetUser(userId);
        }

        public static SocketGuildUser GetGuildUser(ulong userId, ulong guildId)
        {
            return _client.GetGuild(guildId).GetUser(userId);
        }

        public static IMongoDatabase GetDatabase()
        {
            return _database;
        }
    }
}
