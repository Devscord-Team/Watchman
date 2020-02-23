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
            var latestDate = collection.First().CreatedAt;
            var oldestDate = collection.Last().CreatedAt;
            return default;
        }
    }
}
