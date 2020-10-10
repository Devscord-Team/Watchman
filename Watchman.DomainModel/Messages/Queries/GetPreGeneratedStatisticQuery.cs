using System;
using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetPreGeneratedStatisticQuery : PaginationMessagesQuery, IQuery<GetPreGeneratedStatisticsQueryResult>
    {
        public ulong ServerId { get; }
        public ulong ChannelId { get; }
        public ulong UserId { get; }

        public GetPreGeneratedStatisticQuery(ulong serverId, ulong channelId = 0, ulong userId = 0)
        {
            this.ServerId = serverId;
            this.ChannelId = channelId;
            this.UserId = userId;
        }
    }
}
