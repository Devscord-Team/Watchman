using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services.Factories
{
    public class MessagesServiceFactory
    {
        private readonly ResponsesService _responsesService;
        private readonly MessageSplittingService _splittingService;
        private readonly EmbedMessagesService _embedMessagesService;

        public MessagesServiceFactory(ResponsesService responsesService, MessageSplittingService splittingService, EmbedMessagesService embedMessagesService)
        {
            this._responsesService = responsesService;
            _splittingService = splittingService;
            _embedMessagesService = embedMessagesService;
        }

        public MessagesService Create(Contexts contexts)
        {
            return this.Create(contexts.Channel.Id);
        }

        public MessagesService Create(ulong channelId, ulong guildId = 0)
        {
            return new MessagesService(_responsesService, _splittingService, _embedMessagesService)
            {
                GuildId = guildId,
                ChannelId = channelId
            };
        }
    }
}
