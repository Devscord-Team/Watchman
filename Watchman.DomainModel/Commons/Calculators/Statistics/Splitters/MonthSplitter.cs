using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Common.Models;
using Watchman.DomainModel.Messages.Services;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Commons.Calculators.Statistics.Splitters
{
    public class MonthSplitter : ISplitter
    {
        public IEnumerable<KeyValuePair<TimeRange, IEnumerable<T>>> Split<T>(IEnumerable<T> collection) where T : ISplittable
        {
            var groupedByYear = collection.GroupBy(x => x.GetSplittable().Date.Year).Select(x => (Year: x.Key, Month: x.GroupBy(y => y.GetSplittable().Month))).ToList();
            foreach (var year in groupedByYear)
            {
                foreach (var month in year.Month)
                {
                    var timeRange = TimeRange.Create(new DateTime(year.Year, month.Key, 1), new DateTime(year.Year, month.Key + 1, 1).AddMilliseconds(-1));
                    yield return new KeyValuePair<TimeRange, IEnumerable<T>>(timeRange, month);
                }
            }
        }



    }
}
