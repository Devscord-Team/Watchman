using Watchman.Cqrs;

namespace Watchman.DomainModel.Users.Queries
{
    public class GetProtectionPunishmentQueryResult : IQueryResult
    {
        public ProtectionPunishment ProtectionPunishment { get; set; }

        public GetProtectionPunishmentQueryResult(ProtectionPunishment protectionPunishment)
        {
            this.ProtectionPunishment = protectionPunishment;
        }
    }
}
