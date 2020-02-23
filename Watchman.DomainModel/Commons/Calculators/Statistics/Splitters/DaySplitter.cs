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
        public IEnumerable<IGrouping<TimeRange, T>> Split<T>(IEnumerable<T> collection) where T : Entity
        {
            var fullRange = TimeRange.Create(collection.First().CreatedAt.Date, collection.Last().CreatedAt.Date);
            foreach (var index in Enumerable.Range(0, fullRange.DaysBetween))
            {

            }
            return default;
        }
    }
}
