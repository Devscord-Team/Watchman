using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Queries
{
    public class GetDiscordServerSafeRolesQueryResult : IQueryResult
    {
        public IEnumerable<SafeRole> SafeRoles { get; }

        public GetDiscordServerSafeRolesQueryResult(IEnumerable<SafeRole> safeRoles)
        {
            this.SafeRoles = safeRoles;
        }
    }
}
