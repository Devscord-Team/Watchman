using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using System.IO;

namespace Devscord.DiscordFramework.Services
{
    public interface IDirectMessagesService
    {
        Task<bool> TrySendMessage(ulong userId, Func<IResponsesService, string> response, Contexts contexts);
        Task<bool> TrySendMessage(ulong userId, string message, Commons.MessageType messageType = Commons.MessageType.NormalText);
        Task<bool> TrySendEmbedMessage(ulong userId, string title, string description, IEnumerable<KeyValuePair<string, string>> values);
        Task<bool> TrySendEmbedMessage(ulong userId, string title, string description, IEnumerable<KeyValuePair<string, Dictionary<string, string>>> values);
        Task<bool> TrySendFile(ulong userId, string fileName, Stream stream);
    }

    public class DirectMessagesService : IDirectMessagesService
    {
        private readonly IResponsesService _responsesService;
        private readonly MessageSplittingService _messageSplittingService;
        private readonly EmbedMessagesService _embedMessagesService;
        private readonly IEmbedMessageSplittingService _embedMessageSplittingService;

        public DirectMessagesService(IResponsesService responsesService, IMessageSplittingService messageSplittingService, IEmbedMessagesService embedMessagesService, IEmbedMessageSplittingService embedMessageSplittingService)
        {
            this._responsesService = responsesService;
            this._messageSplittingService = messageSplittingService;
            this._embedMessagesService = embedMessagesService;
            this._embedMessageSplittingService = embedMessageSplittingService;
        }

        public Task<bool> TrySendMessage(ulong userId, Func<IResponsesService, string> response, Contexts contexts)
        {
            var message = this._responsesService.GetResponse(contexts.Server.Id, response);
            return this.TrySendMessage(userId, message);
        }

        public async Task<bool> TrySendMessage(ulong userId, string message, Commons.MessageType messageType = Commons.MessageType.NormalText)
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

        public async Task<bool> TrySendFile(ulong userId, string fileName, Stream stream)
        {
            try
            {
                await Server.SendDirectFile(userId, fileName, stream);
                await stream.DisposeAsync();
                Log.Information("Bot sent file {fileName} to user {userId}", fileName, userId);
            }
            catch
            {
                Log.Warning("Cannot send file {fileName} to user {userId}", fileName, userId);
                return false;
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
