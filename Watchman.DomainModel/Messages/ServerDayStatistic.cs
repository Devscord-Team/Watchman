using System;
using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.Commons.Calculators.Statistics.Splitters;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages
{
    public class ServerDayStatistic : Entity, ISplittable
    {
        public ulong ServerId { get; private set; }
        public IEnumerable<ChannelDayStatistic> ChannelDayStatistics { get; private set; }
        public int Count { get; private set; }
        public DateTime Date { get; private set; }

        public ServerDayStatistic(IReadOnlyCollection<Message> serverMessages, ulong serverId, DateTime date)
        {
            ServerId = serverId;
            Date = date;
            Count = serverMessages.Count;
            var channelsMessages = serverMessages.GroupBy(x => x.Channel.Id);

            this.ChannelDayStatistics = channelsMessages.Select(x => new ChannelDayStatistic
            {
                ChannelId = x.Key,
                Count = x.Count()
            });
        }

        public DateTime GetSplittable() => Date;
    }

    public class ChannelDayStatistic
    {
        public ulong ChannelId { get; set; }
        public int Count { get; set; }
    }
}
