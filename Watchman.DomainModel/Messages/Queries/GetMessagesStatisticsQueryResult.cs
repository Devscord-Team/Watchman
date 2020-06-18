using System.Collections.Generic;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages.Services;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetMessagesStatisticsQueryResult : IQueryResult
    {
        public IEnumerable<PeriodStatistic> PeriodStatistics { get; private set; }

        public GetMessagesStatisticsQueryResult(IEnumerable<PeriodStatistic> periodStatistics) => this.PeriodStatistics = periodStatistics;
    }
}
