using Watchman.Common.Models;

namespace Statsman.Models
{
    public class TimeStatisticItem
    {
        public TimeRange Time { get; private set; }
        public int Value { get; private set; }

        public TimeStatisticItem(TimeRange time, int value)
        {
            this.Time = time;
            this.Value = value;
        }
    }
}
