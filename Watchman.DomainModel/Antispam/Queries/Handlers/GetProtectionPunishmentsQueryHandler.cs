﻿using Watchman.Cqrs;
using Watchman.DomainModel.Antispam;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Antispam.Queries.Handlers
{
    public class GetProtectionPunishmentsQueryHandler : IQueryHandler<GetProtectionPunishmentsQuery, GetProtectionPunishmentsQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetProtectionPunishmentsQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetProtectionPunishmentsQueryResult Handle(GetProtectionPunishmentsQuery query)
        {
            using var session = this._sessionFactory.CreateMongo();
            return new GetProtectionPunishmentsQueryResult(session.Get<ProtectionPunishment>());
        }
    }
}
