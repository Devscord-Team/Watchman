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
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System.IO;

namespace Devscord.DiscordFramework.Integration.Services
{
    internal class DiscordClientChannelsService : IDiscordClientChannelsService
    {
        public Func<SocketChannel, Task> ChannelCreated { get; set; }

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

        public async Task SendDirectFile(ulong userId, string fileName, Stream stream)
        {
            var user = await this._discordClientUsersService.GetUser(userId);
            await user.SendFileAsync(stream, fileName);
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

        public async IAsyncEnumerable<Message> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)
        {
            var textChannel = (ITextChannel)this.GetChannel(channel.Id).Result;
            if (!await this.CanBotReadTheChannel(textChannel))
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

        public async Task<bool> CanBotReadTheChannel(IMessageChannel textChannel)
        {
            try
            {
                await textChannel.GetMessagesAsync(limit: 1).FlattenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
