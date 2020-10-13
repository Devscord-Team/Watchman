using System;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Devscord.DiscordFramework.Services.Models;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClientChannelsService : IDiscordClientChannelsService
    {
        public Func<SocketChannel, Task> ChannelCreated { get; set; }
        public Func<SocketChannel, Task> ChannelRemoved { get; set; }

        private DiscordSocketRestClient _restClient => this._client.Rest;
        private readonly DiscordSocketClient _client;
        private readonly IDiscordClientUsersService _discordClientUsersService;
        private readonly UserContextsFactory _userContextsFactory;
        private readonly CommandParser _commandParser;

        public DiscordClientChannelsService(DiscordSocketClient client, IDiscordClientUsersService discordClientUsersService, UserContextsFactory userContextsFactory, CommandParser commandParser)
        {
            this._client = client;
            this._discordClientUsersService = discordClientUsersService;
            this._userContextsFactory = userContextsFactory;
            this._commandParser = commandParser;
            this._client.ChannelCreated += x => this.ChannelCreated(x);
            this._client.ChannelDestroyed += x => this.ChannelRemoved(x);
        }

        public async Task SendDirectMessage(ulong userId, string message)
        {
            var user = await this._discordClientUsersService.GetUser(userId);
            await user.SendMessageAsync(message);
        }

        public async Task SendDirectEmbedMessage(ulong userId, Embed embed)
        {
            var user = await this._discordClientUsersService.GetUser(userId);
            await user.SendMessageAsync(embed: embed);
        }

        public async Task<IChannel> GetChannel(ulong channelId, IGuild guild)
        {
            IChannel channel = await guild.GetChannelAsync(channelId);
            if (channel != null)
            {
                return channel;
            }
            Log.Warning("GetChannel couldn't get channel by channelId from IGuild");
            channel = this._client.GetGuild(guild.Id).GetChannel(channelId);
            if (channel != null)
            {
                return channel;
            }
            Log.Warning("GetChannel couldn't get channel by channelId and serverId from Client");
            channel = await this._restClient.GetChannelAsync(channelId);
            return channel;
        }

        public IChannel GetChannel(ulong channelId, ulong serverId)
        {
            IChannel channel = this._client.GetChannel(channelId);
            if (channel != null)
            {
                return channel;
            }
            Log.Warning("GetChannel couldn't get channel by channelId from Client");
            var guild = this._client.GetGuild(serverId);
            channel = guild.GetChannel(channelId);
            if (channel != null)
            {
                return channel;
            }
            Log.Warning("GetChannel couldn't get channel by channelId and serverId from Client");
            return this._restClient.GetChannelAsync(channelId).GetAwaiter().GetResult(); // it should never go here - maybe it should be possible to remove this code
        }

        public async Task<IGuildChannel> GetGuildChannel(ulong channelId, RestGuild guild = null)
        {
            if (guild != null)
            {
                return await guild.GetChannelAsync(channelId);
            }
            IGuildChannel channel;
            try
            {
                channel = (IGuildChannel)this._client.GetChannel(channelId);
            }
            catch
            {
                channel = (IGuildChannel)await this._restClient.GetChannelAsync(channelId);
                Log.Warning("Client couldn't get channel: {channelId}", channelId);
            }
            return channel;
        }

        public async IAsyncEnumerable<Message> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)
        {
            var textChannel = (ITextChannel)this.GetChannel(channel.Id, server.Id);
            if (!await this.CanBotReadTheChannelAsync(textChannel))
            {
                yield break;
            }
            var channelMessages = fromMessageId == 0 
                ? textChannel.GetMessagesAsync(limit) 
                : textChannel.GetMessagesAsync(fromMessageId, goBefore ? Direction.Before : Direction.After, limit);

            await foreach (var messagesPackage in channelMessages)
            {
                foreach (var message in messagesPackage)
                {
                    var user = this._userContextsFactory.Create(message.Author);
                    var contexts = new Contexts(server, channel, user);

                    DiscordRequest request;
                    try
                    {
                        request = this._commandParser.Parse(message.Content, message.Timestamp.UtcDateTime);
                    }
                    catch // should almost never go to catch block, but in rare cases Parse() can throw an exception
                    {
                        request = new DiscordRequest
                        {
                            OriginalMessage = message.Content, 
                            SentAt = message.Timestamp.UtcDateTime
                        };
                    }
                    yield return new Message(message.Id, request, contexts);
                }
            }
        }

        public async Task<bool> CanBotReadTheChannelAsync(IMessageChannel textChannel)
        {
            var message = await textChannel.GetMessagesAsync(limit: 1).FirstAsync();
            return message.Count != 0;
        }

        public async Task<ITextChannel> CreateNewChannelAsync(ulong serverId, string channelName)
        {
            var guild = await this._restClient.GetGuildAsync(serverId);
            return await guild.CreateTextChannelAsync(channelName);
        }

        public async Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole role)
        {
            Log.Information("Setting role {roleName} for {server}", role.Name, server.Name);
            
            var socketRole = Server.GetSocketRoles(server.Id).FirstOrDefault(x => x.Id == role.Id);
            if (socketRole == null)
            {
                Log.Error("Created role {roleName} was null", role.Name);
                return;
            }
            var channelPermissions = new OverwritePermissions(permissions.AllowPermissions?.GetRawValue() ?? 0, permissions.DenyPermissions?.GetRawValue() ?? 0);
            foreach (var channel in channels)
            {
                Log.Information("Setting role {roleName} for {channel}", role.Name, channel.Name);
                await this.SetRolePermissions(channel, server, channelPermissions, socketRole);
            }
            Log.Information("Successfully set role {roleName} on all channels on {server}", role.Name, server.Name);
        }

        public Task SetRolePermissions(ChannelContext channel, DiscordServerContext server, ChangedPermissions permissions, UserRole role)
        {
            Log.Information("Setting role {roleName} for {channel}", role.Name, channel.Name);

            var channelPermissions = new OverwritePermissions(permissions.AllowPermissions?.GetRawValue() ?? 0, permissions.DenyPermissions?.GetRawValue() ?? 0);
            var socketRole = this.GetSocketRole(server, role);
            return this.SetRolePermissions(channel, server, channelPermissions, socketRole);
        }

        public async Task RemoveRolePermissions(ChannelContext channel, DiscordServerContext server, UserRole role)
        {
            Log.Information("Removing role {roleName} for {channel}", role.Name, channel.Name);

            var socketRole = this.GetSocketRole(server, role);
            var guild = this._client.GetGuild(server.Id);
            var channelSocket = (IGuildChannel)await this.GetChannel(channel.Id, guild);
            await channelSocket.RemovePermissionOverwriteAsync(socketRole);
        }

        private SocketRole GetSocketRole(DiscordServerContext server, UserRole role)
        {
            var socketRole = Server.GetSocketRoles(server.Id).FirstOrDefault(x => x.Id == role.Id);
            if (socketRole == null)
            {
                Log.Error("Role {roleName} was null", role.Name);
            }
            return socketRole;
        }

        private Task SetRolePermissions(ChannelContext channel, DiscordServerContext server, OverwritePermissions permissions, IRole role)
        {
            var guildChannel = (IGuildChannel)this.GetChannel(channel.Id, server.Id);
            if (guildChannel == null)
            {
                Log.Warning("{channel} after casting to IGuildChannel is null", channel.Name);
                return Task.CompletedTask;
            }
            if (guildChannel.PermissionOverwrites.Any(x => x.TargetId == role.Id))
            {
                Log.Warning("Channel {channel} has already assigned this role {roleName}", channel.Name, role.Name);
                return Task.CompletedTask;
            }
            return guildChannel.AddPermissionOverwriteAsync(role, permissions)
                .ContinueWith(_ => Log.Information("{roleName} set for {channel}", role.Name, channel.Name));
        }
    }
}
