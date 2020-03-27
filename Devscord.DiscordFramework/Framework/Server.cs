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

        public static Func<SocketGuildUser, Task> UserJoined { get; set; }

        public static void Initialize(DiscordSocketClient client)
        {
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
            var user = GetUser(userId);
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

        private static Task RoleUpdated(SocketRole from, SocketRole to)
        {
            _roles.Remove(from);
            _roles.Add(to);
            return Task.CompletedTask;
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

        public static async Task SetPermissions(ChannelContext channel, ChangedPermissions permissions, UserRole muteRole)
        {
            await Task.Delay(1000);

            var channelSocket = (IGuildChannel)GetChannel(channel.Id);
            var channelPermissions = new OverwritePermissions(permissions.AllowPermissions.GetRawValue(), permissions.DenyPermissions.GetRawValue());
            var createdRole = Server.GetSocketRoles(channelSocket.GuildId).FirstOrDefault(x => x.Id == muteRole.Id);

            await channelSocket.AddPermissionOverwriteAsync(createdRole, channelPermissions);
        }

        public static async Task SetPermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole muteRole)
        {
            await Task.Delay(1000);
            var createdRole = Server.GetSocketRoles(server.Id).FirstOrDefault(x => x.Id == muteRole.Id);
            var channelPermissions = new OverwritePermissions(permissions.AllowPermissions.GetRawValue(), permissions.DenyPermissions.GetRawValue());

            Parallel.ForEach(channels, c =>
            {
                var channelSocket = (IGuildChannel)GetChannel(c.Id);
                channelSocket.AddPermissionOverwriteAsync(createdRole, channelPermissions);
            });
        }

        public static Task<IEnumerable<DiscordServerContext>> GetDiscordServers()
        {
            var serverContextFactory = new DiscordServerContextFactory();
            var serverContexts = _client.Guilds.Select(x => serverContextFactory.Create(x));
            return Task.FromResult(serverContexts);
        }

        public static async Task<IEnumerable<Message>> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)
        {
            var textChannel = (SocketTextChannel)Server.GetChannel(channel.Id);
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
    }
}
