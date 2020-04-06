using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.DiscordServer.Queries.Handlers
{
    public class GetDiscordServerSafeRolesQueryHandler : IQueryHandler<GetDiscordServerSafeRolesQuery, GetDiscordServerSafeRolesQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetDiscordServerSafeRolesQueryHandler(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public GetDiscordServerSafeRolesQueryResult Handle(GetDiscordServerSafeRolesQuery query)
        {
            var session = _sessionFactory.Create();
            var safeRoles = session.Get<Role>()
                .Where(x => x.ServerId == query.ServerId);
            
            return new GetDiscordServerSafeRolesQueryResult(safeRoles);
        }
    }
}
