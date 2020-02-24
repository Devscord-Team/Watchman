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
            var days = timeRange.DaysBetween;
            var itemsPerDay = quantity / days;
            foreach (var dayIndex in Enumerable.Range(0, days))
            {
                var time = timeRange.Start.AddDays(dayIndex);
                foreach(var itemIndex in Enumerable.Range(0, itemsPerDay))
                {
                    var item = new TestEntity();
                    item.SetCreatedAt(time);
                    item.SetUpdatedAt(time);
                    yield return item;
                }
            }
        }
    }
}
