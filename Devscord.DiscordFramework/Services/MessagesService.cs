using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Integration;
using Discord;
using Discord.Rest;
using Serilog;
using MessageType = Devscord.DiscordFramework.Commons.MessageType;

namespace Devscord.DiscordFramework.Services
{
    public interface IMessagesService
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        Task SendMessage(string message, MessageType messageType = MessageType.NormalText);
        Task SendEmbedMessage(string title, string description, IEnumerable<KeyValuePair<string, string>> values);
        Task SendEmbedMessage(string title, string description, IEnumerable<KeyValuePair<string, Dictionary<string, string>>> values);
        Task SendResponse(Func<IResponsesService, string> response);
        Task SendFile(string filePath);
        Task SendFile(string fileName, Stream stream);
        Task SendExceptionResponse(BotException botException);
    }

    public class MessagesService : IMessagesService
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }

        private readonly IResponsesService _responsesService;
        private readonly IMessageSplittingService _splittingService;
        private readonly IEmbedMessageSplittingService _embedMessageSplittingService;
        private readonly IResponsesCachingService _responsesCachingService;

        public MessagesService(IResponsesService responsesService, IMessageSplittingService splittingService, IEmbedMessageSplittingService embedMessageSplittingService, IResponsesCachingService responsesCachingService)
        {
            this._responsesService = responsesService;
            this._splittingService = splittingService;
            this._embedMessageSplittingService = embedMessageSplittingService;
            this._responsesCachingService = responsesCachingService;
        }

        public async Task SendMessage(string message, MessageType messageType = MessageType.NormalText)
        {
            var channel = this.GetChannel();
            foreach (var mess in this._splittingService.SplitMessage(message, messageType))
            {
                await channel.SendMessageAsync(mess);
                Log.Information("Bot sent message {splitted} {message}", mess, messageType != MessageType.NormalText ? "splitted" : string.Empty);
            }
        }

        public Task SendEmbedMessage(string title, string description, IEnumerable<KeyValuePair<string, string>> values)
        {
            var embeds = this._embedMessageSplittingService.SplitEmbedMessage(title, description, values);
            return this.SendEmbedSplitMessages(embeds);
        }

        public Task SendEmbedMessage(string title, string description, IEnumerable<KeyValuePair<string, Dictionary<string, string>>> values)
        {
            var embeds = this._embedMessageSplittingService.SplitEmbedMessage(title, description, values);
            return this.SendEmbedSplitMessages(embeds);
        }

        public Task SendResponse(Func<IResponsesService, string> response)
        {
            this._responsesService.Responses = this._responsesCachingService.GetResponses(this.GuildId);
            var message = response.Invoke(this._responsesService);
            return this.SendMessage(message);
        }

        public async Task SendFile(string filePath)
        {
            var channel = this.GetChannel();
            await channel.SendFileAsync(filePath);
        }

        public async Task SendFile(string fileName, Stream stream)
        {
            var channel = this.GetChannel();
            await channel.SendFileAsync(stream, fileName);
            await stream.DisposeAsync();
        }

        public Task SendExceptionResponse(BotException botException)
        {
            var responseName = botException.GetType().Name.Replace("Exception", string.Empty);
            var responseManagerMethod = typeof(ResponsesManager).GetMethod(responseName);
            if (responseManagerMethod == null)
            {
                Log.Error("{name} doesn't exists as a response", responseName);
                return this.SendMessage($"{responseName} doesn't exists as a response"); // message typed into code, bcs it's called only when there is a problem with responses
            }
            return this.SendResponse(x =>
            {
                var arg = new object[] { x };
                if (botException.Value != null)
                {
                    arg = arg.Append(botException.Value).ToArray();
                }
                return (string)responseManagerMethod.Invoke(null, arg);
            });
        }

        private IMessageChannel GetChannel() //todo add adapter
        {
            RestGuild guild = null;
            if (this.GuildId != default)
            {
                guild = Server.GetGuild(this.GuildId).Result;
            }
            var channel = (IMessageChannel)Server.GetChannel(this.ChannelId, guild).Result;
            return channel;
        }

        private async Task SendEmbedSplitMessages(IEnumerable<Embed> embeds)
        {
            var channel = this.GetChannel();
            foreach (var embed in embeds)
            {
                await channel.SendMessageAsync(embed: embed);
                Log.Information("Bot sent embed message {description}", embed.Description);
            }
        }
    }
}
