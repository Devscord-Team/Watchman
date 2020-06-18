using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Users.Queries.Handlers
{
    public class GetProtectionPunishmentsQueryHandler : IQueryHandler<GetProtectionPunishmentsQuery, GetProtectionPunishmentsQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetProtectionPunishmentsQueryHandler(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public GetProtectionPunishmentsQueryResult Handle(GetProtectionPunishmentsQuery query)
        {
            using var session = _sessionFactory.Create();
            return new GetProtectionPunishmentsQueryResult(session.Get<ProtectionPunishment>());
        }
    }
}
