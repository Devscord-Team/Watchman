using System;

namespace Watchman.Common.Models
{
    public class TimeRange
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public int DaysBetween => (int)(End - Start).TotalDays;

        public TimeRange()
        {
        }

        public TimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public static TimeRange Create(DateTime start, DateTime end) => new TimeRange(start, end);
        public static TimeRange ToNow(DateTime start) => new TimeRange(start, DateTime.UtcNow);
        public static TimeRange FromNow(DateTime end) => new TimeRange(DateTime.UtcNow, end);
    }
}
