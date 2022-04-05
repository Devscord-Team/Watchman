using System.Collections.Generic;

namespace Watchman.DomainModel.Configuration.ConfigurationItems.Complaints
{
    public class RolesWithAccessToComplaintsChannel : MappedConfiguration<IEnumerable<ulong>>
    {
        public override IEnumerable<ulong> Value { get; set; }
        public override string Group { get; } = "Complaints";

        public RolesWithAccessToComplaintsChannel(ulong serverId) : base(serverId)
        {
        }
    }
}
