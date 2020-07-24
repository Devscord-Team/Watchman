using System.Collections.Generic;
using System.Linq;
using Watchman.Common.Models;

namespace Watchman.DomainModel.Commons.Calculators.Statistics.Splitters
{
    public class DaySplitter : ISplitter
    {
        public IEnumerable<KeyValuePair<TimeRange, IEnumerable<T>>> Split<T>(IEnumerable<T> collection) where T : ISplittable
        {
            var groupedByDay = collection.GroupBy(x => x.GetSplittable().Date);
            foreach (var day in groupedByDay)
            {
                var timeRange = TimeRange.Create(day.Key, day.Key);
                yield return new KeyValuePair<TimeRange, IEnumerable<T>>(timeRange, day);
            }
        }
    }
}
