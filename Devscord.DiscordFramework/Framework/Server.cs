using Devscord.DiscordFramework.Services;
using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Framework
{
    public static class ServerInitializer
    {
        public static bool Initialized = false;

        public static void Initialize(DiscordSocketClient client, string mongoDbConnectionString)
        {
            if (Initialized)
            {
                return;
            }
            Server.Initialize(client, mongoDbConnectionString);
            Initialized = true;
        }
    }


    internal static class Server
    {
        private static DiscordSocketClient _client;
        private static IMongoDatabase _database;

        public static void Initialize(DiscordSocketClient client, string mongoDbConnectionString)
        {
            _client = client;
            _database = new MongoClient(mongoDbConnectionString).GetDatabase("devscord");
            _client.UserJoined += _client_UserJoined;
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

        private static Task _client_UserJoined(SocketGuildUser guildUser)
        {
            var userService = new UserService();
            return Task.CompletedTask;
            //return userService.WelcomeUser();
        }
    }
}
