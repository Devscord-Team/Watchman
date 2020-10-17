using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Users.Queries.Handlers
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
