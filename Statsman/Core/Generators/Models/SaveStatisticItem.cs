using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Common.Models;

namespace Statsman.Core.Generators.Models
{
    public class SaveStatisticItem
    {
        public ulong ServerId { get; }
        public ulong UserId { get; }
        public ulong ChannelId { get; }
        public int Count { get; }
        public TimeRange TimeRange { get; }
        public string Period { get; }

        public SaveStatisticItem(ulong serverId, ulong userId, ulong channelId, int count, TimeRange timeRange, string period)
        {
            this.ServerId = serverId;
            this.UserId = userId;
            this.ChannelId = channelId;
            this.Count = count;
            this.TimeRange = timeRange;
            this.Period = period;
        }
    }
}
