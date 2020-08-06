using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Queries
{
    public class GetServerTrustedRolesQueryResult : IQueryResult
    {
        public IEnumerable<ulong> TrustedRolesIds { get; }

        public GetServerTrustedRolesQueryResult(IEnumerable<ulong> trustedRolesIds)
        {
            this.TrustedRolesIds = trustedRolesIds;
        }
    }
}
