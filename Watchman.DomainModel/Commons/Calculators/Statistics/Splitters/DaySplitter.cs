using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Common.Models;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Commons.Calculators.Statistics.Splitters
{
    public class DaySplitter : ISplitter
    {
        public IEnumerable<KeyValuePair<TimeRange, IEnumerable<T>>> Split<T>(IEnumerable<T> collection) where T : Entity
        {
            var fullRange = TimeRange.Create(collection.First().CreatedAt.Date, collection.Last().CreatedAt.Date);
            var groupedByDay = collection.GroupBy(x => x.CreatedAt.Date)
                .Select(x => );
            foreach (var day in groupedByDay)
            {
            }
        }
    }
}
