using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Common.Models;
using Watchman.DomainModel.Commons.Calculators.Statistics.Splitters;
using Watchman.DomainModel.Messages.Services;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Commons.Calculators.Statistics
{
    public class StatisticsCalculator : IStatisticsCalculator
    {
        public IEnumerable<PeriodStatistic> GetStatisticsPerPeriod<T>(IEnumerable<T> collection, Period period) where T : Entity
        {
            var splitter = this.GetSplitter(period);
            return this.GetStatisticsPerPeriod(collection, splitter);
        }

        public IEnumerable<PeriodStatistic> GetStatisticsPerPeriod<T>(IEnumerable<T> collection, ISplitter splitter) where T : Entity
        {
            var splitted = splitter.Split(collection);
            foreach (var singlePeriod in splitted)
            {
                yield return new PeriodStatistic(singlePeriod.Key, singlePeriod.Value.Count());
            }
        }

        private ISplitter GetSplitter(Period period)
        {
            return period switch
            {
                Period.Hour => new HourSplitter(),
                Period.Day => new DaySplitter(),
                Period.Week => new WeekSplitter(),
                Period.Month => new MonthSplitter(),
                _ => default
            };
        }
    }
}
