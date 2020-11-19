using System;
using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetPreGeneratedStatisticQuery : IQuery<GetPreGeneratedStatisticsQueryResult>
    {
        public ulong ServerId { get; }
        public ulong ChannelId { get; }
        public ulong UserId { get; }
        public string Period { get; }
        public TimeRange TimeRange { get; }

        public GetPreGeneratedStatisticQuery(ulong serverId, ulong channelId = 0, ulong userId = 0, string period = "Day", TimeRange timeRange = null)
        {
            this.ServerId = serverId;
            this.ChannelId = channelId;
            this.UserId = userId;
            this.Period = period;
            this.TimeRange = timeRange;
        }
    }
}
