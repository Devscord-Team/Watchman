using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetPreGeneratedStatisticsQueryResult : IQueryResult
    {
        public IEnumerable<PreGeneratedStatistic> PreGeneratedStatistic { get; }

        public GetPreGeneratedStatisticsQueryResult(IEnumerable<PreGeneratedStatistic> preGeneratedStatistic)
        {
            this.PreGeneratedStatistic = preGeneratedStatistic;
        }
    }
}
