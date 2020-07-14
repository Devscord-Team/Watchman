using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Users.Queries.Handlers
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

            var warnEvents = session.Get<WarnEvent>().ToList();
            List<WarnEvent> filteredEvents = new List<WarnEvent>();

            if (query.ServerId == 0)
            {
                filteredEvents = warnEvents.Where(x => (x.Receiver.Id == query.UserId)).ToList();
            }
            else
            {
                filteredEvents = warnEvents.Where(x => (x.Receiver.Id == query.UserId) && (x.ServerId == query.ServerId)).ToList();
            }

            return new GetWarnEventsQueryResults(filteredEvents);
        }
    }
}
