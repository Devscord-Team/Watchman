using System.Linq;
using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries.Handlers;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Queries.Handlers
{
    public class GetServerDayStatisticsQueryHandler : PaginationQueryHandler, IQueryHandler<GetServerDayStatisticsQuery, GetServerDayStatisticsQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetServerDayStatisticsQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetServerDayStatisticsQueryResult Handle(GetServerDayStatisticsQuery query)
        {
            using var session = this._sessionFactory.Create();
            var statistics = session.Get<ServerDayStatistic>().AsEnumerable();
            statistics = this.Paginate(query, statistics);
            return new GetServerDayStatisticsQueryResult(statistics);
        }
    }
}
