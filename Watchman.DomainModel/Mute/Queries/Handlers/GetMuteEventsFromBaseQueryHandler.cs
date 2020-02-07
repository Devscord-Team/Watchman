using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Mute.Queries.Handlers
{
    public class GetMuteEventsFromBaseQueryHandler : IQueryHandler<GetMuteEventsFromBaseQuery, GetMuteEventsFromBaseQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetMuteEventsFromBaseQueryHandler(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public GetMuteEventsFromBaseQueryResult Handle(GetMuteEventsFromBaseQuery query)
        {
            using var session = _sessionFactory.Create();
            
            var muteEvents = session.Get<MuteEvent>()
                .Where(x => x.ServerId == query.ServerId);

            return new GetMuteEventsFromBaseQueryResult(muteEvents);
        }
    }
}
