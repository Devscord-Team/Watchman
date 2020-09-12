using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Protection.Queries
{
    public class GetProtectionPunishmentsQueryResult : IQueryResult
    {
        public IEnumerable<ProtectionPunishment> ProtectionPunishments { get; }

        public GetProtectionPunishmentsQueryResult(IEnumerable<ProtectionPunishment> protectionPunishments)
        {
            this.ProtectionPunishments = protectionPunishments;
        }
    }
}
