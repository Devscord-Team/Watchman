using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetMessagesQuery : PaginationQuery, IQuery<GetMessagesQueryResult>
    {
        public ulong ServerId { get; private set; }

        public GetMessagesQuery(ulong serverId)
        {
            ServerId = serverId;
        }
    }
}
