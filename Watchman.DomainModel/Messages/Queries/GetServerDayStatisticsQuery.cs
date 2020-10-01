using System;
using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetServerDayStatisticsQuery : PaginationMessagesQuery, IQuery<GetServerDayStatisticsQueryResult>
    {
        public ulong ServerId { get; }
        public ulong ChannelId { get; }
        public ulong UserId { get; }

        public GetServerDayStatisticsQuery(ulong serverId, ulong channelId = 0, ulong userId = 0)
        {
            this.ServerId = serverId;
            this.ChannelId = channelId;
            this.UserId = userId;
        }
    }
}
