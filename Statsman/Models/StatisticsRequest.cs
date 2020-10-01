using System;
using System.Collections.Generic;
using System.Text;

namespace Statsman.Models
{
    public class StatisticsRequest
    {
        public ulong ServerId { get; private set; }
        public TimeSpan TimeBehind { get; private set; }
        public ulong UserId { get; private set; }
        public ulong ChannelId { get; private set; }

        public StatisticsRequest(ulong serverId, TimeSpan timeBehind, ulong userId = 0, ulong channelId = 0)
        {
            this.ServerId = serverId;
            this.TimeBehind = timeBehind;
            this.UserId = userId;
            this.ChannelId = channelId;
        }
    }
}
