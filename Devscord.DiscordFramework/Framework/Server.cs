using System;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Middlewares.Factories;
using Devscord.DiscordFramework.Services.Models;
using Discord.Rest;
using Serilog;

namespace Devscord.DiscordFramework.Framework
{
    internal static class Server
    {
        private static IDiscordClient _discordClient;

        public Func<SocketGuildUser, Task> UserJoined => _discordClient.
        public Func<SocketGuild, Task> BotAddedToServer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<DateTime> ConnectedTimes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<DateTime> DisconnectedTimes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        internal static void Initialize(IDiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public Task AddRole(SocketRole role)
        {
            throw new NotImplementedException();
        }

        public Task BotConnected()
        {
            throw new NotImplementedException();
        }

        public Task BotDisconnected(Exception exception)
        {
            throw new NotImplementedException();
        }

        public bool CanBotReadTheChannel(IMessageChannel textChannel)
        {
            throw new NotImplementedException();
        }

        public Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer)
        {
            throw new NotImplementedException();
        }

        public Task<IChannel> GetChannel(ulong channelId, RestGuild guild = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DiscordServerContext>> GetDiscordServers()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId)
        {
            throw new NotImplementedException();
        }

        public Task<RestGuild> GetGuild(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task<RestGuildUser> GetGuildUser(ulong userId, ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RestGuildUser>> GetGuildUsers(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserRole> GetRoles(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SocketRole> GetSocketRoles(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task<RestUser> GetUser(ulong userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRole(SocketRole role)
        {
            throw new NotImplementedException();
        }

        public Task RoleUpdated(SocketRole from, SocketRole to)
        {
            throw new NotImplementedException();
        }

        public Task SendDirectEmbedMessage(ulong userId, Embed embed)
        {
            throw new NotImplementedException();
        }

        public Task SendDirectMessage(ulong userId, string message)
        {
            throw new NotImplementedException();
        }

        public Task SetRolePermissions(ChannelContext channel, ChangedPermissions permissions, UserRole role)
        {
            throw new NotImplementedException();
        }

        public Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole muteRole)
        {
            throw new NotImplementedException();
        }
    }

    public class DiscordClient : IDiscordClient
    {
        private bool _initialized;
        private DiscordSocketClient _client;

        public IDiscordClientUsersService UsersService { get; private set; }
        public IDiscordClientChannelsService ChannelsService { get; private set; }
        public IDiscordClientRolesService RolesService { get; private set; }
        public IDiscordClientServersService ServersService { get; private set; }

        public DiscordClient(DiscordSocketClient client)
        {
            this._client = client;
            while (client.ConnectionState != ConnectionState.Connected)
            {
                Task.Delay(100).Wait();
            }
            this.Initialize();
        }

        private void Initialize()
        {
            if(_initialized)
            {
                return;
            }
            this.UsersService = new DiscordClientUsersService(this._client);
            this.ChannelsService = new DiscordClientChannelsService(this._client, this.UsersService);
            this.RolesService = new DiscordClientRolesService(this._client, this.ChannelsService);
            this.ServersService = new DiscordClientServersService(this._client, this.ChannelsService);

            _initialized = true;
            Log.Information("DiscordClient initialized");
        }
    }

    public interface IDiscordClient
    {
        IDiscordClientUsersService UsersService { get; set; }
        IDiscordClientChannelsService ChannelsService { get; set; }
        IDiscordClientRolesService RolesService { get; set; }
        IDiscordClientServersService ServersService { get; set; }
    }

    public interface IDiscordClientUsersService
    {
        Func<SocketGuildUser, Task> UserJoined { get; set; }
        Task<RestGuildUser> GetGuildUser(ulong userId, ulong guildId);
        Task<IEnumerable<RestGuildUser>> GetGuildUsers(ulong guildId);
        Task<RestUser> GetUser(ulong userId);
    }

    public interface IDiscordClientChannelsService
    {
        Task<IChannel> GetChannel(ulong channelId, RestGuild guild = null);
        Task SendDirectEmbedMessage(ulong userId, Embed embed);
        Task SendDirectMessage(ulong userId, string message);
    }

    public interface IDiscordClientRolesService
    {
        Task AddRole(SocketRole role);
        Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer);
        IEnumerable<UserRole> GetRoles(ulong guildId);
        IEnumerable<SocketRole> GetSocketRoles(ulong guildId);
        Task RemoveRole(SocketRole role);
        Task RoleUpdated(SocketRole from, SocketRole to);
        Task SetRolePermissions(ChannelContext channel, ChangedPermissions permissions, UserRole role);
        Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole muteRole);
    }

    public interface IDiscordClientServersService
    {
        Func<SocketGuild, Task> BotAddedToServer { get; set; }
        List<DateTime> ConnectedTimes { get; set; }
        List<DateTime> DisconnectedTimes { get; set; }

        Task BotConnected();
        Task BotDisconnected(Exception exception);
        bool CanBotReadTheChannel(IMessageChannel textChannel);
        Task<IEnumerable<DiscordServerContext>> GetDiscordServers();
        Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId);
        Task<RestGuild> GetGuild(ulong guildId);
        Task<IEnumerable<Message>> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true);
    }

    public class DiscordClientRolesService : IDiscordClientRolesService
    {
        private DiscordSocketRestClient _restClient => _client.Rest;
        private readonly DiscordSocketClient _client;
        private readonly IDiscordClientChannelsService _discordClientChannelsService;
        private List<SocketRole> _roles;

        public DiscordClientRolesService(DiscordSocketClient client, IDiscordClientChannelsService discordClientChannelsService)
        {
            this._client = client;
            this._discordClientChannelsService = discordClientChannelsService;

            this._client.Ready += async () => await Task.Run(() =>
            {
                return _roles = this._client.Guilds.SelectMany(x => x.Roles).ToList();
            });
            this._client.RoleCreated += AddRole;
            this._client.RoleDeleted += RemoveRole;
            this._client.RoleUpdated += RoleUpdated;
        }

        public async Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer)
        {
            var permissionsValue = role.Permissions.GetRawValue();

            var guild = await _restClient.GetGuildAsync(discordServer.Id);
            var restRole = await guild.CreateRoleAsync(role.Name, new GuildPermissions(permissionsValue), isMentionable: false);
            var userRole = new UserRoleFactory().Create(restRole);

            return userRole;
        }

        public async Task SetRolePermissions(ChannelContext channel, ChangedPermissions permissions, UserRole role)
        {
            await Task.Delay(1000);

            var channelSocket = (IGuildChannel)await this._discordClientChannelsService.GetChannel(channel.Id);
            var channelPermissions = new OverwritePermissions(permissions.AllowPermissions.GetRawValue(), permissions.DenyPermissions.GetRawValue());
            var createdRole = this.GetSocketRoles(channelSocket.GuildId).FirstOrDefault(x => x.Id == role.Id);

            await channelSocket.AddPermissionOverwriteAsync(createdRole, channelPermissions);
        }

        public async Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole muteRole)
        {
            await Task.Delay(1000);
            var createdRole = this.GetSocketRoles(server.Id).FirstOrDefault(x => x.Id == muteRole.Id);
            var channelPermissions = new OverwritePermissions(permissions.AllowPermissions.GetRawValue(), permissions.DenyPermissions.GetRawValue());

            Parallel.ForEach(channels, async c =>
            {
                var channelSocket = (IGuildChannel)await this._discordClientChannelsService.GetChannel(c.Id);
                await channelSocket.AddPermissionOverwriteAsync(createdRole, channelPermissions);
            });
        }

        public IEnumerable<SocketRole> GetSocketRoles(ulong guildId)
        {
            if (_roles == null) // todo: it should work without this if
                _roles = _client.Guilds.SelectMany(x => x.Roles).ToList();
            return _roles.Where(x => x.Guild.Id == guildId);
        }

        public IEnumerable<UserRole> GetRoles(ulong guildId)
        {
            var roleFactory = new UserRoleFactory();
            return GetSocketRoles(guildId).Select(x => roleFactory.Create(x));
        }

        public Task AddRole(SocketRole role)
        {
            this._roles.Add(role);
            return Task.CompletedTask;
        }

        public Task RemoveRole(SocketRole role)
        {
            this._roles.Remove(role);
            return Task.CompletedTask;
        }

        public Task RoleUpdated(SocketRole from, SocketRole to)
        {
            this._roles.Remove(from);
            this._roles.Add(to);
            return Task.CompletedTask;
        }
    }

    

    public class DiscordClientUsersService : IDiscordClientUsersService
    {
        private DiscordSocketRestClient _restClient => _client.Rest;
        private readonly DiscordSocketClient _client;

        public Func<SocketGuildUser, Task> UserJoined { get; set; }

        public DiscordClientUsersService(DiscordSocketClient client)
        {
            this._client = client;

            this._client.UserJoined += user => this.UserJoined(user);
        }

        public async Task<RestUser> GetUser(ulong userId)
        {
            return await this._restClient.GetUserAsync(userId);
        }

        public async Task<RestGuildUser> GetGuildUser(ulong userId, ulong guildId)
        {
            var guild = await _restClient.GetGuildAsync(guildId);
            return await guild.GetUserAsync(userId);
        }

        public async Task<IEnumerable<RestGuildUser>> GetGuildUsers(ulong guildId)
        {
            var guild = await _restClient.GetGuildAsync(guildId);
            var users = guild.GetUsersAsync();
            return await users.FlattenAsync();
        }
    }

    

    public class DiscordClientChannelsService : IDiscordClientChannelsService
    {
        private DiscordSocketRestClient _restClient => _client.Rest;
        private readonly DiscordSocketClient _client;
        private readonly IDiscordClientUsersService _discordClientUsersService;

        public DiscordClientChannelsService(DiscordSocketClient client, IDiscordClientUsersService discordClientUsersService)
        {
            this._client = client;
            this._discordClientUsersService = discordClientUsersService;
        }

        public async Task SendDirectMessage(ulong userId, string message)
        {
            var user = await _discordClientUsersService.GetUser(userId);
            await user.SendMessageAsync(message);
        }

        public async Task SendDirectEmbedMessage(ulong userId, Embed embed)
        {
            var user = await _discordClientUsersService.GetUser(userId);
            await user.SendMessageAsync(embed: embed);
        }

        public async Task<IChannel> GetChannel(ulong channelId, RestGuild guild = null)
        {
            if (guild != null)
            {
                return await guild.GetChannelAsync(channelId);
            }

            IChannel channel;
            try
            {
                channel = await _restClient.GetChannelAsync(channelId);
            }
            catch
            {
                Log.Warning($"RestClient couldn't get channel: {channelId}");
                channel = _client.GetChannel(channelId);
            }
            return channel;
        }
    }

    

    public class DiscordClientServersService : IDiscordClientServersService
    {
        private DiscordSocketRestClient _restClient => _client.Rest;
        private readonly DiscordSocketClient _client;
        private readonly IDiscordClientChannelsService _discordClientChannelsService;

        public Func<SocketGuild, Task> BotAddedToServer { get; set; }
        public List<DateTime> DisconnectedTimes { get; set; } = new List<DateTime>();
        public List<DateTime> ConnectedTimes { get; set; } = new List<DateTime>();

        public DiscordClientServersService(DiscordSocketClient client, IDiscordClientChannelsService discordClientChannelsService)
        {
            this._client = client;
            this._discordClientChannelsService = discordClientChannelsService;

            this._client.JoinedGuild += this.BotAddedToServer;
            this._client.Disconnected += this.BotDisconnected;
            this._client.Connected += this.BotConnected;
        }

        public async Task<RestGuild> GetGuild(ulong guildId)
        {
            return await _restClient.GetGuildAsync(guildId);
        }

        public async Task<IEnumerable<DiscordServerContext>> GetDiscordServers()
        {
            var serverContextFactory = new DiscordServerContextFactory();
            var guilds = await _restClient.GetGuildsAsync();
            var serverContexts = guilds.Select(x => serverContextFactory.Create(x));
            return serverContexts;
        }

        public async Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId)
        {
            var guild = this._client.GetGuild(serverId);
            var invites = await guild.GetInvitesAsync();
            return invites.Select(x => x.Url);
        }

        public async Task<IEnumerable<Message>> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)
        {
            var textChannel = (ITextChannel)this._discordClientChannelsService.GetChannel(channel.Id).Result;
            if (!this.CanBotReadTheChannel(textChannel))
            {
                return new List<Message>();
            }

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

        public bool CanBotReadTheChannel(IMessageChannel textChannel)
        {
            try
            {
                textChannel.GetMessagesAsync(limit: 1).FlattenAsync().Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task BotDisconnected(Exception exception)
        {
            Log.Warning(exception, "Bot disconnected!");
            this.DisconnectedTimes.Add(DateTime.Now);
            return Task.CompletedTask;
        }

        public Task BotConnected()
        {
            this.ConnectedTimes.Add(DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
