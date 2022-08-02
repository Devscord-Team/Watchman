using System;

namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestTimeItem : MappedConfiguration<DateTime>
    {
        public override DateTime Value { get; set; } = default;
        public override string Group { get; set; } = "TestItems";

        public TestTimeItem(ulong serverId) : base(serverId)
        {
        }
    }
}
