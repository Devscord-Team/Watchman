using System;

namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestTimeItem : MappedConfiguration<TimeSpan>
    {
        public override TimeSpan Value { get; set; } = default;
        public override string Group { get; set; } = "TestItems";

        public TestTimeItem(ulong serverId) : base(serverId)
        {
        }
    }
}
