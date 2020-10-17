using Microsoft.VisualBasic.CompilerServices;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

        public bool Contains(DateTime dateTime)
        {
            return dateTime >= this.Start && dateTime <= this.End;
        }

        public TimeRange Move(TimeSpan time)
        {
            var clone = this.Clone();
            clone.Start = this.Start.Add(time);
            clone.End = this.End.Add(time);
            return clone;
        }

        public TimeRange Clone()
        {
            return new TimeRange(this.Start, this.End);
        }

        public IEnumerable<TimeRange> MoveWhile(Func<TimeRange, bool> shouldContinue, Func<TimeRange,TimeSpan> getTime)
        {
            return this.MoveWhile(shouldContinue, getTime.Invoke(this));
        }

        public IEnumerable<TimeRange> MoveWhile(Func<TimeRange, bool> shouldContinue, TimeSpan time)
        {
            var timeRange = this;
            while(shouldContinue.Invoke(timeRange))
            {
                yield return this;
                timeRange = this.Move(time);
            }
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

        public static bool operator ==(TimeRange a, TimeRange b)
        {
            var aHash = a?.GetHashCode() ?? 0;
            var bHash = b?.GetHashCode() ?? 0;
            if(aHash == bHash)
            {
                return true;
            }
            if(aHash == 0 || bHash == 0)
            {
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(TimeRange a, TimeRange b)
            => !(a == b);

        public TimeRange ForeachMinute(Action<int, DateTime> action)
        {
            this.Foreach(this.MinutesBetween, this.Start.AddMinutes, action);
            return this;
        }

        public TimeRange ForeachHour(Action<int, DateTime> action)
        {
            this.Foreach(this.HoursBetween, this.Start.AddHours, action);
            return this;
        }

        public TimeRange ForeachDay(Action<int, DateTime> action)
        {
            this.Foreach(this.DaysBetween, this.Start.AddDays, action);
            return this;
        }

        private void Foreach(int loop, Func<double, DateTime> add, Action<int, DateTime> action)
        {
            for (var i = 0; i <= loop; i++)
            {
                action.Invoke(i, add.Invoke(i));
            }
        }

        public override bool Equals(object obj)
        {
            return obj is TimeRange range && this.Start == range.Start && this.End == range.End;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Start, this.End);
        }
    }
}
