using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries;

namespace Watchman.DomainModel.Messages.Queries.Handlers
{
    public class GetServerDayStatisticsQuery : PaginationQuery, IQuery<GetServerDayStatisticsQueryResult>
    {
        public ulong ServerId { get; }

        public GetServerDayStatisticsQuery(ulong serverId)
        {
            this.ServerId = serverId;
        }
    }
}
