using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Warnings.Queries.Handlers
{
    public class GetWarnEventsQueryHandler : IQueryHandler<GetWarnEventsQuery, GetWarnEventsQueryResults>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetWarnEventsQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetWarnEventsQueryResults Handle(GetWarnEventsQuery query)
        {
            using var session = this._sessionFactory.Create();

            if (query.ServerId == 0)
            {
                var filteredEvents = session.Get<WarnEvent>().Where(x => x.ReceiverId == query.UserId);
                return new GetWarnEventsQueryResults(filteredEvents);
            }
            else
            {
                var filteredEvents = session.Get<WarnEvent>().Where(x => x.ReceiverId == query.UserId && x.ServerId == query.ServerId);
                return new GetWarnEventsQueryResults(filteredEvents);
            }
        }
    }
}
