using System.Linq;
using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries.Handlers;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Messages.Queries.Handlers
{
    public class GetPreGeneratedStatisticQueryHandler : IQueryHandler<GetPreGeneratedStatisticQuery, GetPreGeneratedStatisticsQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetPreGeneratedStatisticQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetPreGeneratedStatisticsQueryResult Handle(GetPreGeneratedStatisticQuery query)
        {
            using var session = this._sessionFactory.CreateLite();
            var preGeneratedStatistics = session.Get<PreGeneratedStatistic>().Where(x => x.ServerId == query.ServerId && x.Period == query.Period);
            if (query.TimeRange != null)
            {
                preGeneratedStatistics = preGeneratedStatistics.Where(x => query.TimeRange.Contains(x.TimeRange.Start) && query.TimeRange.Contains(x.TimeRange.End));
            }
            if (query.UserId != 0)
            {
                preGeneratedStatistics = preGeneratedStatistics.Where(x => x.UserId == query.UserId);
            }
            if (query.ChannelId != 0)
            {
                preGeneratedStatistics = preGeneratedStatistics.Where(x => x.ChannelId == query.ChannelId);
            }
            return new GetPreGeneratedStatisticsQueryResult(preGeneratedStatistics.ToList());
        }
    }
}
