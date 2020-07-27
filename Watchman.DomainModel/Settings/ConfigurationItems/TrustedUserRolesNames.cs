using System.Collections.Generic;

namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class TrustedUserRolesNames : MappedConfiguration<IEnumerable<string>>
    {
        public override IEnumerable<string> Value { get; set; } = new List<string>();

        public TrustedUserRolesNames(ulong serverId) : base(serverId)
        {
        }
    }
}
