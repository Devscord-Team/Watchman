using Devscord.DiscordFramework.Middlewares;
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


    internal static class Server
    {
        private static DiscordSocketClient _client;

        public static void Initialize(DiscordSocketClient client)
        {
            _client = client;
            _client.UserJoined += UserJoined;
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

        //todo there should be command (command handler)
        private static Task UserJoined(SocketGuildUser guildUser)
        {
            var channelContext = (new ChannelContextFactory()).Create(guildUser.Guild.DefaultChannel);
            var userContext = (new UserContextsFactory()).Create(guildUser);
            var discordServerContext = (new DiscordServerContextFactory()).Create(guildUser.Guild);

            var userService = new UserService();
            return userService.WelcomeUser(channelContext, userContext, discordServerContext);
        }
    }
}
