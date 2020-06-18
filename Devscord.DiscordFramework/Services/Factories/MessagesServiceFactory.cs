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
            this._splittingService = splittingService;
            this._embedMessagesService = embedMessagesService;
        }

        public MessagesService Create(Contexts contexts) => this.Create(contexts.Channel.Id);

        public MessagesService Create(ulong channelId, ulong guildId = 0)
        {
            return new MessagesService(this._responsesService, this._splittingService, this._embedMessagesService)
            {
                GuildId = guildId,
                ChannelId = channelId
            };
        }
    }
}
