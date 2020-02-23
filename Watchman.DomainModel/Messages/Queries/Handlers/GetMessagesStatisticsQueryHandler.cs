using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Calculators.Statistics;
using Watchman.DomainModel.Commons.Queries.Handlers;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Queries.Handlers
{
    public class GetMessagesStatisticsQueryHandler : PaginationQueryHandler, IQueryHandler<GetMessagesStatisticsQuery, GetMessagesStatisticsQueryResult>
    {
        private readonly ISessionFactory sessionFactory;
        private readonly IStatisticsCalculator statisticsCalculator;

        public GetMessagesStatisticsQueryHandler(ISessionFactory sessionFactory, IStatisticsCalculator statisticsCalculator)
        {
            this.sessionFactory = sessionFactory;
            this.statisticsCalculator = statisticsCalculator;
        }

        public GetMessagesStatisticsQueryResult Handle(GetMessagesStatisticsQuery query)
        {
            using (var session = this.sessionFactory.Create())
            {
                var messages = session.Get<Message>();
                var paginated = this.Paginate(query, messages);
                var statistics = this.statisticsCalculator.GetStatisticsPerPeriod(paginated, query.Period);
                return new GetMessagesStatisticsQueryResult(statistics);
            }
        }
    }
}
