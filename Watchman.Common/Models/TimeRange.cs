using System;

namespace Watchman.Common.Models
{
    public class TimeRange
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public int MinutesBetween => (int) (this.End - this.Start).TotalMinutes;
        public int HoursBetween => (int) (this.End - this.Start).TotalHours;
        public int DaysBetween => (int) (this.End - this.Start).TotalDays;

        public TimeRange()
        {
        }

        public TimeRange(DateTime start, DateTime end)
        {
            this.Start = start;
            this.End = end;
        }

        public static TimeRange Create(DateTime start, DateTime end)
        {
            return new TimeRange(start, end);
        }

        public static TimeRange ToNow(DateTime start)
        {
            return new TimeRange(start, DateTime.UtcNow);
        }

        public static TimeRange FromNow(DateTime end)
        {
            return new TimeRange(DateTime.UtcNow, end);
        }

        public override string ToString()
        {
            var timezone = TimeZoneInfo.Local;
            if (this.DaysBetween <= 7)
            {
                return $"{this.Start.ToLocalTime():dd/MM/yyyy HH:mm} - {this.End.ToLocalTime():dd/MM/yyyy HH:mm} (UTC+{timezone.BaseUtcOffset.TotalHours}:00)";
            }
            return $"{this.Start.ToLocalTime():dd/MM/yyyy} - {this.End.ToLocalTime():dd/MM/yyyy} (UTC+{timezone.BaseUtcOffset.TotalHours}:00)";
        }

        public void ForeachMinute(Action<int, DateTime> action)
        {
            this.Foreach(this.MinutesBetween, this.Start.AddMinutes, action);
        }

        public void ForeachHour(Action<int, DateTime> action)
        {
            this.Foreach(this.HoursBetween, this.Start.AddHours, action);
        }

        public void ForeachDay(Action<int, DateTime> action)
        {
            this.Foreach(this.DaysBetween, this.Start.AddDays, action);
        }

        private void Foreach(int loop, Func<double, DateTime> add, Action<int, DateTime> action)
        {
            for (var i = 0; i <= loop; i++)
            {
                action.Invoke(i, add.Invoke(i));
            }
        }
    }
}
