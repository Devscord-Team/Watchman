using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Watchman.Common.Models;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages
{
    public class PreGeneratedStatistic : Entity, IAggregateRoot
    {
        public ulong ServerId { get; private set; }
        public ulong UserId { get; private set; }
        public ulong ChannelId { get; private set; }
        public int Count { get; private set; }
        public TimeRange TimeRange { get; private set; }
        public string Period { get; private set; }

        public PreGeneratedStatistic(ulong serverId, int count, TimeRange timeRange, Func<Period, string> getPeriod)
        {
            this.ServerId = serverId;
            this.Count = count;
            this.TimeRange = timeRange;
            this.Period = getPeriod.Invoke(new Period());
        }
    }

    public class Period
    {
        public const string Day = "Day";
        public const string Month = "Month";
        public const string Quarter = "Quarter";
    }
}
