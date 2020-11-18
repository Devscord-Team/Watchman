using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services.Factories
{
    public class MessagesServiceFactory
    {
        private readonly ResponsesService _responsesService;
        private readonly MessageSplittingService _splittingService;
        private readonly EmbedMessageSplittingService _embedMessageSplittingService;
        private readonly ResponsesCachingService _responsesCachingService;

        public MessagesServiceFactory(ResponsesService responsesService, MessageSplittingService splittingService, EmbedMessageSplittingService embedMessageSplittingService, ResponsesCachingService responsesCachingService)
        {
            this._responsesService = responsesService;
            this._splittingService = splittingService;
            this._embedMessageSplittingService = embedMessageSplittingService;
            this._responsesCachingService = responsesCachingService;
        }

        public MessagesService Create(Contexts contexts)
        {
            return this.Create(contexts.Channel.Id, contexts.Server.Id);
        }

        public MessagesService Create(ulong channelId, ulong guildId)
        {
            return new MessagesService(this._responsesService, this._splittingService, this._embedMessageSplittingService, this._responsesCachingService)
            {
                GuildId = guildId,
                ChannelId = channelId
            };
        }
    }
}
