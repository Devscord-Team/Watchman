using System.Collections.Generic;
using Watchman.Cqrs;
using Watchman.DomainModel.Antispam;

namespace Watchman.DomainModel.Antispam.Queries
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
