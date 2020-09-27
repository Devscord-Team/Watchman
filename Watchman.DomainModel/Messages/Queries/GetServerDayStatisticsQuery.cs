using System;
using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetServerDayStatisticsQuery : PaginationMessagesQuery, IQuery<GetServerDayStatisticsQueryResult>
    {
        public ulong ServerId { get; }

        public GetServerDayStatisticsQuery(ulong serverId)
        {
            this.ServerId = serverId;
        }
    }
}
