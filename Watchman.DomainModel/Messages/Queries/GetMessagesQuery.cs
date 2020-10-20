using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetMessagesQuery : PaginationMessagesQuery, IQuery<GetMessagesQueryResult>
    {
        public const ulong GET_ALL_SERVERS = 0;

        public ulong ServerId { get; private set; }
        public ulong ChannelId { get; private set; }
        public ulong UserId { get; private set; }

        public GetMessagesQuery(ulong serverId, ulong channelId = 0, ulong userId = 0)
        {
            this.ServerId = serverId;
            this.ChannelId = channelId;
            this.UserId = userId;
        }
    }
}
