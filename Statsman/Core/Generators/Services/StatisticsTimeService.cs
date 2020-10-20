using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Watchman.Common.Models;
using Watchman.DomainModel.Messages;

namespace Statsman.Core.Generators.Services
{
    public class StatisticsTimeService
    {
        public IEnumerable<TimeRange> GetTimeRangeMovePerPeriod(string period, DateTime oldestMessageDatetime) //TODO test
        {
            var startOfCurrentPeriod = this.GetStartOfCurrentPeriod(period);
            var moveForward = this.GetMoveForward(period);
            var moveBackward = this.GetMoveBackward(period);
            var daysAtEnd = this.GetDaysAtEnd(period);

            return TimeRange.Create(startOfCurrentPeriod, startOfCurrentPeriod.AddDays(moveForward.Invoke(startOfCurrentPeriod) - daysAtEnd).AddMilliseconds(-1))
                .Move(TimeSpan.FromDays(-moveBackward.Invoke(startOfCurrentPeriod)))
                .MoveWhile(x => !x.Contains(oldestMessageDatetime), x => TimeSpan.FromDays(-moveBackward.Invoke(x.Start)));
        }

        public DateTime GetStartOfCurrentPeriod(string period)
        {
            return period switch
            {
                Period.Day => DateTime.Today,
                Period.Month => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                Period.Quarter => this.GetQuarterStart(DateTime.Today),
                _ => throw new NotImplementedException()
            };
        }

        public Func<DateTime, int> GetMoveForward(string period)
        {
            return period switch
            {
                Period.Day => new Func<DateTime, int>(x => 1),
                Period.Month => new Func<DateTime, int>(x => DateTime.DaysInMonth(x.Year, x.Month)),
                Period.Quarter => new Func<DateTime, int>(x => new DateTime[] { x, x.AddMonths(1), x.AddMonths(2) }.Select(d => DateTime.DaysInMonth(d.Year, d.Month)).Sum()),
                _ => throw new NotImplementedException()
            };
        }

        public Func<DateTime, int> GetMoveBackward(string period)
        {
            return period switch
            {
                Period.Day => new Func<DateTime, int>(x => 1),
                Period.Month => new Func<DateTime, int>(x => DateTime.DaysInMonth(x.AddMonths(-1).Year, x.AddMonths(-1).Month)),
                Period.Quarter => new Func<DateTime, int>(x => new DateTime[] { x.AddMonths(-1), x.AddMonths(-2), x.AddMonths(-3) }.Select(d => DateTime.DaysInMonth(d.Year, d.Month)).Sum()),
                _ => throw new NotImplementedException()
            };
        }

        public int GetDaysAtEnd(string period)
        {
            return period switch
            {
                Period.Day => 0,
                Period.Month => 1,
                Period.Quarter => 1,
                _ => throw new NotImplementedException()
            };
        }

        public DateTime GetQuarterStart(DateTime date)
        {
            return date.Month switch
            {
                int month when month <= 3 => new DateTime(date.Year, 1, 1),
                int month when month <= 6 => new DateTime(date.Year, 4, 1),
                int month when month <= 9 => new DateTime(date.Year, 7, 1),
                _ => new DateTime(date.Year, 10, 1)
            };
        }
    }
}
