using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Settings.Queries.Handlers
{
    public class GetInitEventsQueryHandler : IQueryHandler<GetInitEventsQuery, GetInitEventsQueryResults>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetInitEventsQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetInitEventsQueryResults Handle(GetInitEventsQuery query)
        {
            using var session = this._sessionFactory.CreateMongo();
            var initEvents = session.Get<InitEvent>()
                .Where(x => x.ServerId == query.ServerId);

            return new GetInitEventsQueryResults(initEvents);
        }
    }
}
