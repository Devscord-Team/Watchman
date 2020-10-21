using System.Collections.Generic;

namespace Watchman.DomainModel.Configuration.ConfigurationItems
{
    public class RolesWithAccessToComplaintsChannel : MappedConfiguration<IEnumerable<ulong>>
    {
        public override IEnumerable<ulong> Value { get; set; }
        
        public RolesWithAccessToComplaintsChannel(ulong serverId) : base(serverId)
        {
        }
    }
}
