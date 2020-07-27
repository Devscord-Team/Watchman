using System.Collections.Generic;

namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class SafeUserRolesNames : MappedConfiguration<IEnumerable<string>>
    {
        public override IEnumerable<string> Value { get; set; } = new List<string>();

        public SafeUserRolesNames(ulong serverId) : base(serverId)
        {
        }
    }
}
