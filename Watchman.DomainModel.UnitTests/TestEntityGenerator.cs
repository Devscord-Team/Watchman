using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Common.Models;

namespace Watchman.DomainModel.UnitTests
{
    public static class TestEntityGenerator
    {
        public static IEnumerable<TestEntity> Generate(TimeRange timeRange, int quantity)
        {
            var ticksBetweenTimes = timeRange.End.Ticks - timeRange.Start.Ticks;
            var ticksPerItem = (long)Math.Truncate((decimal)ticksBetweenTimes / quantity);
            foreach (var index in Enumerable.Range(0, quantity))
            {
                var item = new TestEntity();
                var time = timeRange.Start.AddTicks(ticksPerItem * index);
                item.SetCreatedAt(time);
                item.SetUpdatedAt(time);
                yield return item;
            }
        }
    }
}
