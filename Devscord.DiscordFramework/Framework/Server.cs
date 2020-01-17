using System;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Factories;

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

    internal static class Server
    {
        private static DiscordSocketClient _client;
        private static List<SocketRole> _roles;

        public static IEnumerable<SocketRole> GetRoles(ulong guildId)
        {
            return _roles.Where(x => x.Guild.Id == guildId);
        }

        private static Task AddRole(SocketRole role)
        {
            _roles.Add(role);
            return Task.CompletedTask;
        }

        private static Task RemoveRole(SocketRole role)
        {
            _roles.Remove(role);
            return Task.CompletedTask;
        }

        public static void Initialize(DiscordSocketClient client)
        {
            _client = client;
            _client.UserJoined += UserJoined;

            client.Ready += () =>
            {
                _roles = client.Guilds.SelectMany(x => x.Roles).ToList();
                return Task.CompletedTask;
            };

            _client.RoleCreated += (newRole) => AddRole(newRole);
            _client.RoleDeleted += (deletedRole) => RemoveRole(deletedRole);
        }

        public static SocketChannel GetChannel(ulong channelId)
        {
            return _client.GetChannel(channelId);
        }

        public static SocketUser GetUser(ulong userId)
        {
            return _client.GetUser(userId);
        }

        public static SocketGuildUser GetGuildUser(ulong userId, ulong guildId)
        {
            return _client.GetGuild(guildId).GetUser(userId);
        }

        public static IReadOnlyCollection<SocketGuildUser> GetGuildUsers(ulong guildId)
        {
            return _client.GetGuild(guildId).Users;
        }

        //todo there should be command (command handler)
        private static Task UserJoined(SocketGuildUser guildUser)
        {
            var userContext = new UserContextsFactory().Create(guildUser);
            var discordServerContext = new DiscordServerContextFactory().Create(guildUser.Guild);
            var systemChannel = new ChannelContextFactory().Create(guildUser.Guild.SystemChannel);

            var contexts = new Contexts();
            contexts.SetContext(userContext);
            contexts.SetContext(discordServerContext);
            contexts.SetContext(systemChannel);
          
            var messagesService = new MessagesServiceFactory(new ResponsesService(), new MessageSplittingService()).Create(contexts);

            var userService = new UsersService();
            return userService.WelcomeUser(messagesService, contexts);
        }

        public static Task<UserRole> CreateNewRole(UserRole role, DiscordServerContext discordServer)
        {
            var permissionsValue = role.Permissions.RawValue;

            var createRoleTask = _client.GetGuild(discordServer.Id)
                .CreateRoleAsync(role.Name, new GuildPermissions(permissionsValue));

            var restRole = createRoleTask.Result;
            var userRole = new UserRoleFactory().Create(restRole);

            return Task.FromResult(userRole);
        }

        public static Task SetPermissions(ChannelContext channel, ChangedPermissions permissions, UserRole muteRole)
        {
            var channelSocket = (IGuildChannel)GetChannel(channel.Id);
            var channelPermissions = new OverwritePermissions(permissions.AllowPermissions.RawValue, permissions.DenyPermissions.RawValue);

            Task SetPerms(SocketRole createdRole)
            {
                if (createdRole.Id != muteRole.Id)
                    return Task.CompletedTask;

                _client.RoleCreated -= SetPerms;
                return channelSocket.AddPermissionOverwriteAsync(createdRole, channelPermissions);
            }
            _client.RoleCreated += SetPerms;

            return Task.CompletedTask;
        }
    }
}
