using System.Linq;

using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries.Handlers;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Queries.Handlers
{
    public class GetPreGeneratedStatisticQueryHandler : PaginationMessagesQueryHandler, IQueryHandler<GetPreGeneratedStatisticQuery, GetPreGeneratedStatisticsQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetPreGeneratedStatisticQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetPreGeneratedStatisticsQueryResult Handle(GetPreGeneratedStatisticQuery query)
        {
            using var session = this._sessionFactory.Create();
            var statistics = session.Get<PreGeneratedStatistic>().AsEnumerable();
            statistics = this.Paginate(query, statistics);
            if (query.ChannelId != 0)
            {
                foreach (var statisticItem in statistics)
                {
                    var channelStats = statisticItem.ChannelDayStatistics.FirstOrDefault(x => x.ChannelId == query.ChannelId);
                    if(channelStats == null)
                    {
                        statisticItem.SetCount(0);
                    }
                    else
                    {
                        statisticItem.SetCount(channelStats.Count);
                    }
                }
            }
            if (query.UserId != 0)
            {
                messages = this.TakeOnlyForUser(query.UserId.Value, messages);
            }
            return new GetPreGeneratedStatisticsQueryResult(statistics);
        }
    }
}
