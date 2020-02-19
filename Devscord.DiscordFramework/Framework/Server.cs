using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Factories;

namespace Devscord.DiscordFramework.Framework
{
    public static class ServerInitializer
    {
        public static bool Initialized { get; private set; }

        public static void Initialize(DiscordSocketClient client, MessagesServiceFactory messagesServiceFactory)
        {
            if (Initialized)
            {
                return;
            }
            Server.Initialize(client, messagesServiceFactory);
            Initialized = true;
        }
    }

    internal static class Server
    {
        private static DiscordSocketClient _client;
        private static List<SocketRole> _roles;
        
        public static IEnumerable<SocketRole> GetSocketRoles(ulong guildId)
        {
            return _roles.Where(x => x.Guild.Id == guildId);
        }

        public static IEnumerable<UserRole> GetRoles(ulong guildId)
        {
            var roleFactory = new UserRoleFactory();
            return GetSocketRoles(guildId).Select(x => roleFactory.Create(x));
        }

        public static void Initialize(DiscordSocketClient client, MessagesServiceFactory messagesServiceFactory)
        {
            _client = client;
            _client.UserJoined += user => UserJoined(user, messagesServiceFactory);

            client.Ready += () =>
            {
                _roles = client.Guilds.SelectMany(x => x.Roles).ToList();
                return Task.CompletedTask;
            };

            _client.RoleCreated += newRole => AddRole(newRole);
            _client.RoleDeleted += deletedRole => RemoveRole(deletedRole);
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

        //todo there should be command (command handler)
        private static Task UserJoined(SocketGuildUser guildUser, MessagesServiceFactory messagesServiceFactory)
        {
            var userContext = new UserContextsFactory().Create(guildUser);
            var discordServerContext = new DiscordServerContextFactory().Create(guildUser.Guild);
            var landingChannel = discordServerContext.LandingChannel;

            var contexts = new Contexts();
            contexts.SetContext(userContext);
            contexts.SetContext(discordServerContext);
            contexts.SetContext(landingChannel);

            var userService = new UsersService(messagesServiceFactory);
            return userService.WelcomeUser(contexts);
        }

        public static Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer)
        {
            var permissionsValue = role.Permissions.GetRawValue();

            var createRoleTask = _client.GetGuild(discordServer.Id)
                .CreateRoleAsync(role.Name, new GuildPermissions(permissionsValue));

            var restRole = createRoleTask.Result;
            var userRole = new UserRoleFactory().Create(restRole);

            return Task.FromResult(userRole);
        }

        public static Task SetPermissions(ChannelContext channel, ChangedPermissions permissions, UserRole muteRole)
        {
            var channelSocket = (IGuildChannel)GetChannel(channel.Id);
            var channelPermissions = new OverwritePermissions(permissions.AllowPermissions.GetRawValue(), permissions.DenyPermissions.GetRawValue());

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

        public static Task<IEnumerable<DiscordServerContext>> GetDiscordServers()
        {
            var serverContextFactory = new DiscordServerContextFactory();
            var serverContexts = _client.Guilds.Select(x => serverContextFactory.Create(x));
            return Task.FromResult(serverContexts);
        }
    }
}
