using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetPreGeneratedStatisticsQueryResult : IQueryResult
    {
        public IEnumerable<PreGeneratedStatistic> PreGeneratedStatistics { get; }

        public GetPreGeneratedStatisticsQueryResult(IEnumerable<PreGeneratedStatistic> preGeneratedStatistics)
        {
            this.PreGeneratedStatistics = preGeneratedStatistics;
        }
    }
}
