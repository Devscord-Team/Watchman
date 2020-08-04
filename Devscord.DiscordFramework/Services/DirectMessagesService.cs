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

        public DirectMessagesService(ResponsesService responsesService, MessageSplittingService messageSplittingService, EmbedMessagesService embedMessagesService, EmbedMessageSplittingService embedMessageSplittingService)
        {
            this._responsesService = responsesService;
            this._messageSplittingService = messageSplittingService;
            this._embedMessagesService = embedMessagesService;
            this._embedMessageSplittingService = embedMessageSplittingService;
        }

        public Task<bool> TrySendMessage(ulong userId, Func<ResponsesService, string> response, Contexts contexts)
        {
            this._responsesService.RefreshResponses(contexts.Server.Id);
            var message = response.Invoke(this._responsesService);
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

        public async Task<bool> TrySendEmbedMessage(ulong userId, string title, string description, IEnumerable<KeyValuePair<string, string>> values)
        {
            var embed = this._embedMessagesService.Generate(title, description, values);
            return await TrySendEmbedMessage(userId, embed);
        }

        public async Task<bool> TrySendEmbedMessage(ulong userId, string title, string description, Dictionary<string, Dictionary<string, string>> values)
        {
            var embed = this._embedMessagesService.Generate(title, description, values);
            return await TrySendEmbedMessage(userId, embed);
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
