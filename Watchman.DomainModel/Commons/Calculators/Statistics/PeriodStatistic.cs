using Watchman.Common.Models;

namespace Watchman.DomainModel.Messages.Services
{
    public class PeriodStatistic
    {
        public TimeRange TimeRange { get; private set; }
        public int Count { get; private set; }

        public PeriodStatistic(TimeRange timeRange, int count)
        {
            this.TimeRange = timeRange;
            this.Count = count;
        }
    }
}
