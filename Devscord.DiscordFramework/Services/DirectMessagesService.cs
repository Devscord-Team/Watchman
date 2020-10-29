using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using MessageType = Devscord.DiscordFramework.Commons.MessageType;

namespace Devscord.DiscordFramework.Services
{
    public class DirectMessagesService
    {
        private readonly ResponsesService _responsesService;
        private readonly MessageSplittingService _messageSplittingService;
        private readonly EmbedMessagesService _embedMessagesService;
        private readonly EmbedMessageSplittingService _embedMessageSplittingService;
        private readonly ResponsesCachingService _responsesCachingService;

        public DirectMessagesService(ResponsesService responsesService, MessageSplittingService messageSplittingService, EmbedMessagesService embedMessagesService, EmbedMessageSplittingService embedMessageSplittingService, ResponsesCachingService responsesCachingService)
        {
            this._responsesService = responsesService;
            this._messageSplittingService = messageSplittingService;
            this._embedMessagesService = embedMessagesService;
            this._embedMessageSplittingService = embedMessageSplittingService;
            this._responsesCachingService = responsesCachingService;
        }

        public Task<bool> TrySendMessage(ulong userId, Func<ResponsesService, string> response, Contexts contexts)
        {
            this._responsesService.Responses = this._responsesCachingService.GetResponses(contexts.Server.Id);
            var message = response.Invoke(this._responsesService);
            //var message = this._responsesService.GetResponse(contexts.Server.Id, response);
            return this.TrySendMessage(userId, message);
        }

        public async Task<bool> TrySendMessage(ulong userId, string message, MessageType messageType = MessageType.NormalText)
        {
            try
            {
                foreach (var smallMessages in this._messageSplittingService.SplitMessage(message, messageType))
                {
                    await Server.SendDirectMessage(userId, smallMessages);
                    Log.Information("Bot sent message {smallMessages}", smallMessages);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message, ex);
                return false;
            }
        }

        public Task<bool> TrySendEmbedMessage(ulong userId, string title, string description, IEnumerable<KeyValuePair<string, string>> values)
        {
            var splitEmbedMessages = this._embedMessageSplittingService.SplitEmbedMessage(title, description, values);
            return this.TrySendEmbedSplitMessages(userId, splitEmbedMessages);
        }

        public Task<bool> TrySendEmbedMessage(ulong userId, string title, string description, IEnumerable<KeyValuePair<string, Dictionary<string, string>>> values)
        {
            var splitEmbedMessages = this._embedMessageSplittingService.SplitEmbedMessage(title, description, values);
            return this.TrySendEmbedSplitMessages(userId, splitEmbedMessages);
        }

        private async Task<bool> TrySendEmbedSplitMessages(ulong userId, IEnumerable<Embed> embeds)
        {
            foreach (var embed in embeds)
            {
                if (!await this.TrySendEmbedMessage(userId, embed))
                {
                    return false;
                }
                Log.Information("Bot sent embed message {description}", embed.Description);
            }
            return true;
        }

        private async Task<bool> TrySendEmbedMessage(ulong userId, Embed embed)
        {
            try
            {
                await Server.SendDirectEmbedMessage(userId, embed);
                Log.Information("Bot sent embed message {messageTitle}", embed.Title);
                return true;
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message, ex);
                return false;
            }
        }
    }
}
