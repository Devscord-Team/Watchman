using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public class DirectMessagesService
    {
        private readonly ResponsesService _responsesService;

        public DirectMessagesService(ResponsesService responsesService)
        {
            this._responsesService = responsesService;
        }

        public Task<bool> TrySendMessage(ulong userId, Func<ResponsesService, string> response, Contexts contexts)
        {
            _responsesService.RefreshResponses(contexts);
            var message = response.Invoke(this._responsesService);
            return this.TrySendMessage(userId, message);
        }

        public async Task<bool> TrySendMessage(ulong userId, string message)
        {
            try
            {
                await Server.SendDirectMessage(userId, message);
                Log.Information($"Bot sent message {message}");
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
