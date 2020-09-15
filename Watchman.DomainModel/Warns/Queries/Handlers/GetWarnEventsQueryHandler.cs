using System.Linq;
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
            var filteredEvents = session.Get<WarnEvent>();
            if (query.ReceiverId != 0)
            {
                filteredEvents = filteredEvents.Where(x => x.ReceiverId == query.ReceiverId);
            }
            if (query.ServerId != 0)
            {
                filteredEvents = filteredEvents.Where(x => x.ServerId == query.ServerId);
            }
            filteredEvents = filteredEvents.Where(x => x.CreatedAt >= query.From);
            return new GetWarnEventsQueryResults(filteredEvents);
        }
    }
}
