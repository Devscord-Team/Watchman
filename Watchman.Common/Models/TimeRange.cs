using System;

namespace Watchman.Common.Models
{
    public class TimeRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public TimeRange()
        {
        }

        public TimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public static TimeRange Create(DateTime start, DateTime end) => new TimeRange(start, end);
    }
}
