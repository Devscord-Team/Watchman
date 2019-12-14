using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Queries
{
    public class GetDiscordServerSafeRolesQueryResult : IQueryResult
    {
        public IEnumerable<Role> SafeRoles { get; }

        public GetDiscordServerSafeRolesQueryResult(IEnumerable<Role> safeRoles)
        {
            this.SafeRoles = safeRoles;
        }
    }
}
