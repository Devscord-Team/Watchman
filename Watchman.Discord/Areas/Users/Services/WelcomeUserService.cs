using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System.Threading.Tasks;

namespace Watchman.Discord.Areas.Users.Services
{
    public class WelcomeUserService
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;

<<<<<<< HEAD
        public WelcomeUserService(MessagesServiceFactory messagesServiceFactory)
        {
            this._messagesServiceFactory = messagesServiceFactory;
        }
=======
        public WelcomeUserService(MessagesServiceFactory messagesServiceFactory)
        {
            this._messagesServiceFactory = messagesServiceFactory;
        }
>>>>>>> master
        public Task WelcomeUser(Contexts contexts)
        {
            if (contexts.Channel == null)
            {
                return Task.CompletedTask;
            }

            var messagesService = this._messagesServiceFactory.Create(contexts);
<<<<<<< HEAD
            messagesService.SendResponse(x => x.NewUserArrived(contexts), contexts);
=======
            messagesService.SendResponse(x => x.NewUserArrived(contexts));
>>>>>>> master
            return Task.CompletedTask;
        }
    }
}
