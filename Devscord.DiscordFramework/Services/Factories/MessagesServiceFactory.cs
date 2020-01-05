using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services.Factories
{
    public class MessagesServiceFactory : IService
    {
        private readonly ResponsesService _responsesService;
        private readonly MessageSplittingService _splittingService;

        public MessagesServiceFactory(ResponsesService responsesService, MessageSplittingService splittingService)
        {
            this._responsesService = responsesService;
            _splittingService = splittingService;
        }

        public MessagesService Create(Contexts contexts)
        {
            return new MessagesService(_responsesService, _splittingService)
            {
                ChannelId = contexts.Channel.Id
            };
        }
    }
}
