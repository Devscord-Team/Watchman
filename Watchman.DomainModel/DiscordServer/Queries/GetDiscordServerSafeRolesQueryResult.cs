using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Queries
{
    public class GetDiscordServerSafeRolesQueryResult : IQueryResult
    {
        public IEnumerable<Role> SafeRoles { get; }

        public GetDiscordServerSafeRolesQueryResult(IEnumerable<Role> safeRoles) => this.SafeRoles = safeRoles;
    }
}
