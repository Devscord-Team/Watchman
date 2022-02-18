using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System.Threading.Tasks;

namespace Watchman.Discord.Areas.Users.Services
{
    public interface IWelcomeUserService
    {
        Task WelcomeUser(Contexts contexts);
    }

    public class WelcomeUserService : IWelcomeUserService
    {
        private readonly IMessagesServiceFactory _messagesServiceFactory;

        public WelcomeUserService(IMessagesServiceFactory messagesServiceFactory)
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
