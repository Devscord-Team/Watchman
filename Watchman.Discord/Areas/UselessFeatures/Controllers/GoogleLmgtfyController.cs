using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.UselessFeatures.BotCommands;

namespace Watchman.Discord.Areas.UselessFeatures.Controllers
{
    public class GoogleLmgtfyController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private const string LINK_TO_LMGTFY = "https://pl.lmgtfy.com/?q=";

        public GoogleLmgtfyController(MessagesServiceFactory messagesServiceFactory)
        {
            this._messagesServiceFactory = messagesServiceFactory;
        }

        public Task PrintGoogleLmgtfy(GoogleCommand googleCommand, Contexts contexts)
        {
            if (string.IsNullOrWhiteSpace(googleCommand.Search))
            {
                throw new NotEnoughArgumentsException();
            }
            var searchLink = LINK_TO_LMGTFY + googleCommand.Search.Replace(' ', '+');
            var messagesService = this._messagesServiceFactory.Create(contexts);
            return messagesService.SendResponse(x => x.TryToGoogleIt(searchLink));
        }
    }
}
