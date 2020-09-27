using System;
using System.Collections.Generic;
using System.Linq;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages
{
    public class ServerDayStatistic : Entity
    {
        public ulong ServerId { get; private set; }
        public IEnumerable<ChannelDayStatistic> ChannelDayStatistics { get; private set; }
        public int Count { get; private set; }
        public DateTime Date { get; private set; }

        public ServerDayStatistic(IReadOnlyCollection<Message> serverMessages, ulong serverId, DateTime date)
        {
            this.ServerId = serverId;
            this.Date = date;
            this.Count = serverMessages.Count;
            var channelsMessages = serverMessages.GroupBy(x => x.Channel.Id);

            this.ChannelDayStatistics = channelsMessages.Select(x => new ChannelDayStatistic
            {
                ChannelId = x.Key,
                Count = x.Count()
            });
        }
    }

    public class ChannelDayStatistic
    {
        public ulong ChannelId { get; set; }
        public int Count { get; set; }
    }
}
