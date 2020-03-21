using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Calculators.Statistics;
using Watchman.DomainModel.Commons.Queries.Handlers;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Queries.Handlers
{
    public class GetMessagesStatisticsQueryHandler : PaginationQueryHandler, IQueryHandler<GetMessagesStatisticsQuery, GetMessagesStatisticsQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly IStatisticsCalculator _statisticsCalculator;

        public GetMessagesStatisticsQueryHandler(ISessionFactory sessionFactory, IStatisticsCalculator statisticsCalculator)
        {
            this._sessionFactory = sessionFactory;
            this._statisticsCalculator = statisticsCalculator;
        }

        public GetMessagesStatisticsQueryResult Handle(GetMessagesStatisticsQuery query)
        {
            using var session = this._sessionFactory.Create();
            var messages = session.Get<Message>();
            var paginated = this.Paginate(query, messages);
            var statistics = this._statisticsCalculator.GetStatisticsPerPeriod(paginated, query.Period);
            return new GetMessagesStatisticsQueryResult(statistics);
        }
    }
}
