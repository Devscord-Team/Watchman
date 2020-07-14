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

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClientChannelsService : IDiscordClientChannelsService
    {
        public Func<SocketChannel, Task> ChannelCreated { get; set; }

        private DiscordSocketRestClient _restClient => this._client.Rest;
        private readonly DiscordSocketClient _client;
        private readonly IDiscordClientUsersService _discordClientUsersService;

        public DiscordClientChannelsService(DiscordSocketClient client, IDiscordClientUsersService discordClientUsersService)
        {
            this._client = client;
            this._discordClientUsersService = discordClientUsersService;
            this._client.ChannelCreated += x => this.ChannelCreated(x);
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

        public async Task<IChannel> GetChannel(ulong channelId, RestGuild guild = null)
        {
            if (guild != null)
            {
                return await guild.GetChannelAsync(channelId);
            }

            IChannel channel;
            try
            {
                channel = this._client.GetChannel(channelId);
            }
            catch
            {
                channel = await this._restClient.GetChannelAsync(channelId);
                Log.Warning("RestClient couldn't get channel: {channelId}", channelId);
            }
            return channel;
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
                Log.Warning("RestClient couldn't get channel: {channelId}", channelId);
            }
            return channel;
        }

        public async Task<IEnumerable<Message>> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)
        {
            var textChannel = (ITextChannel)this.GetChannel(channel.Id).Result;
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
    }
}
