using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Common.Models;

namespace Watchman.DomainModel.UnitTests
{
    public static class TestEntityGenerator
    {
        //TODO add generate randomly
        //=> items per month in timerange
        //=> items per day in timerange
        //=> items per hour in timerange

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
                    yield return item;
                }
            }
        }

        public static IEnumerable<TestEntity> Generate(TimeRange timeRange, Func<int, TestEntity> func)
        {
            foreach (var dayIndex in Enumerable.Range(0, timeRange.DaysBetween))
            {
                var result = func.Invoke(dayIndex);
                if (result != null)
                    yield return result;
            }
        }
    }
}
