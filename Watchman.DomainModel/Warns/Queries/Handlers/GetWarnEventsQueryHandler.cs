using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Warns.Queries.Handlers
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
            var filteredEvents = session.Get<WarnEvent>().Where(x => x.ReceiverId == query.UserId);
            if (query.ServerId != 0)
            {
                filteredEvents = filteredEvents.Where(x => x.ServerId == query.ServerId);
            }
            return new GetWarnEventsQueryResults(filteredEvents);
        }
    }
}
