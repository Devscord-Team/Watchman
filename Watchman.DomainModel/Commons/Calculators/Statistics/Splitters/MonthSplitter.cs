using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Common.Models;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Commons.Calculators.Statistics.Splitters
{
    public class MonthSplitter : ISplitter
    {
        public IEnumerable<IGrouping<TimeRange, T>> Split<T>(IEnumerable<T> collection) where T : Entity
        {
            throw new NotImplementedException();
        }
    }
}
