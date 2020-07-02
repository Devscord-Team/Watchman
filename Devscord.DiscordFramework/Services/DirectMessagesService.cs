using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Integration;

namespace Devscord.DiscordFramework.Services
{
    public class DirectMessagesService
    {
        private readonly ResponsesService _responsesService;
        private readonly MessageSplittingService _messageSplittingService;

        public DirectMessagesService(ResponsesService responsesService, MessageSplittingService messageSplittingService)
        {
            this._responsesService = responsesService;
            _messageSplittingService = messageSplittingService;
        }

        public Task<bool> TrySendMessage(ulong userId, Func<ResponsesService, string> response, Contexts contexts)
        {
            _responsesService.RefreshResponses(contexts.Server.Id);
            var message = response.Invoke(this._responsesService);
            return this.TrySendMessage(userId, message);
        }

        public async Task<bool> TrySendMessage(ulong userId, string message, MessageType messageType = MessageType.NormalText)
        {
            try
            {
                foreach (var smallMessages in _messageSplittingService.SplitMessage(message, messageType))
                {
                    await Server.SendDirectMessage(userId, smallMessages);
                    Log.Information($"Bot sent message {smallMessages}");
                }
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
