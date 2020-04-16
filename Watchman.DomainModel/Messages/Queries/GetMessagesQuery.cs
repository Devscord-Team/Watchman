using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetMessagesQuery : PaginationMessagesQuery, IQuery<GetMessagesQueryResult>
    {
        public ulong ServerId { get; private set; }

        public GetMessagesQuery(ulong serverId)
        {
            ServerId = serverId;
        }
    }
}
