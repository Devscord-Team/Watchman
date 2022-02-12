using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System.Threading.Tasks;

namespace Watchman.Discord.Areas.Users.Services
{
    public class WelcomeUserService
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public WelcomeUserService(MessagesServiceFactory messagesServiceFactory)
        {
            this._messagesServiceFactory = messagesServiceFactory;
        }

        public Task WelcomeUser(Contexts contexts)
        {
            if (contexts.Channel == null)
            {
                return Task.CompletedTask;
            }

            var messagesService = this._messagesServiceFactory.Create(contexts);
            messagesService.SendResponse(x => x.NewUserArrived(contexts));
            return Task.CompletedTask;
        }
    }
}
