using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services.Factories
{
    public interface IMessagesServiceFactory
    {
        IMessagesService Create(Contexts contexts);
        IMessagesService Create(ulong channelId, ulong guildId);
    }

    public class MessagesServiceFactory : IMessagesServiceFactory
    {
        private readonly IResponsesService _responsesService;
        private readonly IMessageSplittingService _splittingService;
        private readonly IEmbedMessageSplittingService _embedMessageSplittingService;
        private readonly IResponsesCachingService _responsesCachingService;

        public MessagesServiceFactory(IResponsesService responsesService, IMessageSplittingService splittingService, IEmbedMessageSplittingService embedMessageSplittingService, IResponsesCachingService responsesCachingService)
        {
            this._responsesService = responsesService;
            this._splittingService = splittingService;
            this._embedMessageSplittingService = embedMessageSplittingService;
            this._responsesCachingService = responsesCachingService;
        }

        public IMessagesService Create(Contexts contexts)
        {
            return this.Create(contexts.Channel.Id, contexts.Server.Id);
        }

        public IMessagesService Create(ulong channelId, ulong guildId)
        {
            return new MessagesService(this._responsesService, this._splittingService, this._embedMessageSplittingService, this._responsesCachingService)
            {
                GuildId = guildId,
                ChannelId = channelId
            };
        }
    }
}
