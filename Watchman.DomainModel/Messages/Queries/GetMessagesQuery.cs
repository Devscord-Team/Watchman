using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetMessagesQuery : PaginationMessagesQuery, IQuery<GetMessagesQueryResult>
    {
        public ulong ServerId { get; private set; }
        public ulong ChannelId { get; private set; }
        public ulong? UserId { get; private set; }

        public GetMessagesQuery(ulong serverId) => this.ServerId = serverId;

        public GetMessagesQuery(ulong serverId, ulong? userId)
        {
            this.ServerId = serverId;
            this.UserId = userId;
        }

        public GetMessagesQuery(ulong serverId, ulong channelId, ulong? userId)
        {
            this.ServerId = serverId;
            this.ChannelId = channelId;
            this.UserId = userId;
        }
    }
}
