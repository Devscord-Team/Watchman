using System.Collections.Generic;
using Watchman.Common.Models;
using Watchman.DomainModel.Commons.Calculators.Statistics.Splitters;
using Watchman.DomainModel.Messages.Services;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Commons.Calculators.Statistics
{
    public interface IStatisticsCalculator
    {
        IEnumerable<PeriodStatistic> GetStatisticsPerPeriod<T>(IEnumerable<T> collection, Period period) where T : ISplittable;
        public IEnumerable<PeriodStatistic> GetStatisticsPerPeriod<T>(IEnumerable<T> collection, ISplitter splitter) where T : ISplittable;
    }
}
