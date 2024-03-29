﻿using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Muting.Queries.Handlers
{
    public class GetMuteEventsQueryHandler : IQueryHandler<GetMuteEventsQuery, GetMuteEventsQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetMuteEventsQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetMuteEventsQueryResult Handle(GetMuteEventsQuery query)
        {
            using var session = this._sessionFactory.CreateMongo();
            var muteEvents = session.Get<MuteEvent>()
                .Where(x => x.ServerId == query.ServerId);
            if (query.TakeOnlyNotUnmuted)
            {
                muteEvents = muteEvents.Where(x => !x.IsUnmuted);
            }
            if (query.UserId.HasValue)
            {
                muteEvents = muteEvents.Where(x => x.UserId == query.UserId);
            }
            return new GetMuteEventsQueryResult(muteEvents);
        }
    }
}
