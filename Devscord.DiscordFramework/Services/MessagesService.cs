using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.Rest;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public class MessagesService
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }

        private readonly ResponsesService _responsesService;
        private readonly MessageSplittingService _splittingService;
        private readonly EmbedMessagesService _embedMessagesService;

        public MessagesService(ResponsesService responsesService, MessageSplittingService splittingService, EmbedMessagesService embedMessagesService)
        {
            _responsesService = responsesService;
            _splittingService = splittingService;
            _embedMessagesService = embedMessagesService;
        }

        public Task SendMessage(string message, MessageType messageType = MessageType.NormalText)
        {
            var channel = this.GetChannel();
            foreach (var mess in _splittingService.SplitMessage(message, messageType))
            {
                channel.SendMessageAsync(mess);
                Log.Information("Bot sent message {splitted} {message}", mess, messageType != MessageType.NormalText ? "splitted" : string.Empty);
            }

            return Task.CompletedTask;
        }

        public Task SendEmbedMessage(string title, string description, IEnumerable<KeyValuePair<string, string>> values)
        {
            var channel = this.GetChannel();
            var embed = this._embedMessagesService.Generate(title, description, values);
            channel.SendMessageAsync(embed: embed);
            return Task.CompletedTask;
        }

        public Task SendResponse(Func<ResponsesService, string> response, Contexts contexts)
        {
            _responsesService.RefreshResponses(contexts);
            var message = response.Invoke(this._responsesService);
            return this.SendMessage(message);
        }

        public async Task SendFile(string filePath)
        {
            var channel = (IRestMessageChannel) await Server.GetChannel(ChannelId);
            await channel.SendFileAsync(filePath);
        }

        private IRestMessageChannel GetChannel()
        {
            RestGuild guild = null;
            if (GuildId != default)
                guild = Server.GetGuild(GuildId).Result;
            var channel = (IRestMessageChannel)Server.GetChannel(ChannelId, guild).Result;
            return channel;
        }
    }
}
