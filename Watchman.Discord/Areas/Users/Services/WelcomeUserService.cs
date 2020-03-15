using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;

namespace Watchman.Discord.Areas.Users.Services
{
    public class WelcomeUserService
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public WelcomeUserService(MessagesServiceFactory messagesServiceFactory)
        {
            _messagesServiceFactory = messagesServiceFactory;
        }

        public Task WelcomeUser(Contexts contexts)
        {
            var messagesService = _messagesServiceFactory.Create(contexts);
            messagesService.SendResponse(x => x.NewUserArrived(contexts), contexts);
            return Task.CompletedTask;
        }
    }
}
