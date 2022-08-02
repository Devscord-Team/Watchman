namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestBoolItem : MappedConfiguration<bool>
    {
        public override bool Value { get; set; } = true;
        public override string Group { get; set; } = "TestItems";

        public TestBoolItem(ulong serverId) : base(serverId)
        {
        }
    }
}
