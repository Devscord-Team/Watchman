using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Messaging.Administration.BotCommands;

namespace Watchman.Discord.Areas.Messaging.Administration.Controllers
{
    public class SendController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        
        public SendController(MessagesServiceFactory messagesServiceFactory)
        {
            this._messagesServiceFactory = messagesServiceFactory;
        }

        [AdminCommand]
        public async Task Send(SendCommand sendCommand, Contexts contexts)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            messagesService.ChannelId = sendCommand.Channel;
            await messagesService.SendMessage(sendCommand.Message);
        }
    }
}
