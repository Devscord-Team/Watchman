using System;
using System.Collections;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Factories;
using Devscord.DiscordFramework.Services.Models;
using Discord.Rest;

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
        private static DiscordSocketRestClient _restClient => _client.Rest; //try to use only rest client if it is possible, because it is safer in our architecture
        private static DiscordSocketClient _client;
        private static List<SocketRole> _roles;
        public static Func<SocketGuildUser, Task> UserJoined { get; set; }

        public static void Initialize(DiscordSocketClient client)
        {
            while(client.ConnectionState != ConnectionState.Connected)
            { }

            _client = client;
            _client.UserJoined += user => UserJoined(user);

            _client.Ready += async () => await Task.Run(() =>
            {
                return _roles = client.Guilds.SelectMany(x => x.Roles).ToList();
            });

            _client.RoleCreated += AddRole;
            _client.RoleDeleted += RemoveRole;
            _client.RoleUpdated += RoleUpdated;
        }

        public static async Task SendDirectMessage(ulong userId, string message)
        {
            var user = await GetUser(userId);
            await user.SendMessageAsync(message);
        }

        public static IEnumerable<SocketRole> GetSocketRoles(ulong guildId)
        {
            if (_roles == null) // todo: it should work without this if
                _roles = _client.Guilds.SelectMany(x => x.Roles).ToList();
            return _roles.Where(x => x.Guild.Id == guildId);
        }

        public static IEnumerable<UserRole> GetRoles(ulong guildId)
        {
            var roleFactory = new UserRoleFactory();
            return GetSocketRoles(guildId).Select(x => roleFactory.Create(x));
        }

        public static async Task<RestChannel> GetChannel(ulong channelId, RestGuild guild = null)
        {
            if (guild != null)
                return await guild.GetChannelAsync(channelId);
            return await _restClient.GetChannelAsync(channelId);
        }

        public static async Task<RestUser> GetUser(ulong userId)
        {
            return await _restClient.GetUserAsync(userId);
        }

        public static async Task<RestGuild> GetGuild(ulong guildId)
        {
            return await _restClient.GetGuildAsync(guildId);
        }

        public static async Task<RestGuildUser> GetGuildUser(ulong userId, ulong guildId)
        {
            var guild = await _restClient.GetGuildAsync(guildId);
            return await guild.GetUserAsync(userId);
        }

        public static async Task<IEnumerable<RestGuildUser>> GetGuildUsers(ulong guildId)
        {
            var guild = await _restClient.GetGuildAsync(guildId);
            var users = guild.GetUsersAsync();
            return await users.FlattenAsync();
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

        private static Task RoleUpdated(SocketRole from, SocketRole to)
        {
            _roles.Remove(from);
            _roles.Add(to);
            return Task.CompletedTask;
        }

        public static async Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer)
        {
            var permissionsValue = role.Permissions.GetRawValue();

            var guild = await _restClient.GetGuildAsync(discordServer.Id);
            var createRoleTask = guild.CreateRoleAsync(role.Name, new GuildPermissions(permissionsValue));

            var restRole = createRoleTask.Result;
            var userRole = new UserRoleFactory().Create(restRole);

            return userRole;
        }

        public static async Task SetPermissions(ChannelContext channel, ChangedPermissions permissions, UserRole muteRole)
        {
            await Task.Delay(1000);

            var channelSocket = (IGuildChannel)await GetChannel(channel.Id);
            var channelPermissions = new OverwritePermissions(permissions.AllowPermissions.GetRawValue(), permissions.DenyPermissions.GetRawValue());
            var createdRole = Server.GetSocketRoles(channelSocket.GuildId).FirstOrDefault(x => x.Id == muteRole.Id);

            await channelSocket.AddPermissionOverwriteAsync(createdRole, channelPermissions);
        }

        public static async Task SetPermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole muteRole)
        {
            await Task.Delay(1000);
            var createdRole = Server.GetSocketRoles(server.Id).FirstOrDefault(x => x.Id == muteRole.Id);
            var channelPermissions = new OverwritePermissions(permissions.AllowPermissions.GetRawValue(), permissions.DenyPermissions.GetRawValue());

            Parallel.ForEach(channels, async c =>
            {
                var channelSocket = (IGuildChannel) await GetChannel(c.Id);
                await channelSocket.AddPermissionOverwriteAsync(createdRole, channelPermissions);
            });
        }

        public static async Task<IEnumerable<DiscordServerContext>> GetDiscordServers()
        {
            var serverContextFactory = new DiscordServerContextFactory();
            var guilds = await _restClient.GetGuildsAsync();
            var serverContexts = guilds.Select(x => serverContextFactory.Create(x));
            return serverContexts;
        }

        public static async Task<IEnumerable<Message>> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)
        {
            var textChannel = (RestTextChannel)Server.GetChannel(channel.Id).Result;
            IEnumerable<IMessage> channelMessages;
            if (fromMessageId == 0)
            {
                channelMessages = await textChannel.GetMessagesAsync(limit).FlattenAsync();
            }
            else
            {
                channelMessages = await textChannel.GetMessagesAsync(fromMessageId, goBefore ? Direction.Before : Direction.After, limit).FlattenAsync();
            }

            var userFactory = new UserContextsFactory();
            var messages = channelMessages.Select(message =>
            {
                var user = userFactory.Create(message.Author);
                var contexts = new Contexts();
                contexts.SetContext(server);
                contexts.SetContext(channel);
                contexts.SetContext(user);

                var commandParser = new CommandParser();
                var request = commandParser.Parse(message.Content, message.Timestamp.UtcDateTime);
                return new Message(message.Id, request, contexts);
            });
            return messages;
        }

        public static async Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId)
        {
            var guild = _client.GetGuild(serverId);
            var invites = await guild.GetInvitesAsync();
            return invites.Select(x => x.Url);
        }
    }
}
