using System.Linq;

using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.DiscordServer.Queries.Handlers
{
    public class GetServerTrustedRolesQueryHandler : IQueryHandler<GetServerTrustedRolesQuery, GetServerTrustedRolesQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetServerTrustedRolesQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetServerTrustedRolesQueryResult Handle(GetServerTrustedRolesQuery query)
        {
            using var session = this._sessionFactory.CreateMongo();
            var rolesIds = session.Get<TrustedRole>().Where(x => x.ServerId == query.ServerId);
            return new GetServerTrustedRolesQueryResult(rolesIds.Select(x => x.RoleId));
        }
    }
}
