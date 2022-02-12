using System.Threading.Tasks;
using System.Web;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.UselessFeatures.BotCommands;

namespace Watchman.Discord.Areas.UselessFeatures.Controllers
{
    public class GoogleController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private const string GOOGLE_BASE_URL = "http://letmegooglethat.com/?q=";

        public GoogleController(MessagesServiceFactory messagesServiceFactory)
        {
            this._messagesServiceFactory = messagesServiceFactory;
        }

        public Task PrintGoogle(GoogleCommand googleCommand, Contexts contexts)
        {
            var searchLink = GOOGLE_BASE_URL + HttpUtility.UrlEncode(googleCommand.Search);
            var messagesService = this._messagesServiceFactory.Create(contexts);
            return messagesService.SendResponse(x => x.TryToGoogleIt(searchLink));
        }
    }
}
