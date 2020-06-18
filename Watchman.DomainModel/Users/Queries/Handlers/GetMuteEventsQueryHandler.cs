using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Users.Queries.Handlers
{
    public class GetMuteEventsQueryHandler : IQueryHandler<GetMuteEventsQuery, GetMuteEventsQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetMuteEventsQueryHandler(ISessionFactory sessionFactory) => this._sessionFactory = sessionFactory;

        public GetMuteEventsQueryResult Handle(GetMuteEventsQuery query)
        {
            using var session = this._sessionFactory.Create();

            var muteEvents = session.Get<MuteEvent>()
                .Where(x => x.ServerId == query.ServerId);

            return new GetMuteEventsQueryResult(muteEvents);
        }
    }
}
