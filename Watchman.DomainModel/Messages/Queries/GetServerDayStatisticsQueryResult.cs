using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetServerDayStatisticsQueryResult : IQueryResult
    {
        public IEnumerable<ServerDayStatistic> ServerDayStatistics { get; }

        public GetServerDayStatisticsQueryResult(IEnumerable<ServerDayStatistic> serverDayStatistics) => this.ServerDayStatistics = serverDayStatistics;
    }
}
