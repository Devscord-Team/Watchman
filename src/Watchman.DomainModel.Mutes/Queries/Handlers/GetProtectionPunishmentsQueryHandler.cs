using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Mutes.Queries.Handlers
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
            using var session = this._sessionFactory.Create();
            return new GetProtectionPunishmentsQueryResult(session.Get<ProtectionPunishment>());
        }
    }
}
