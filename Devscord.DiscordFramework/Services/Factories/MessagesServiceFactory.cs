using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services.Factories
{
    public class MessagesServiceFactory : IService
    {
        private readonly ResponsesService responsesService;

        public MessagesServiceFactory(ResponsesService responsesService)
        {
            this.responsesService = responsesService;
        }

        public MessagesService Create(Contexts contexts)
        {
            return new MessagesService(responsesService)
            {
                ChannelId = contexts.Channel.Id
            };
        }
    }
}
