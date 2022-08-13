namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestNullableBoolItem : MappedConfiguration<bool?>
    {
        public override bool? Value { get; set; } = true;
        public override string Group { get; set; } = "TestItems";

        public TestNullableBoolItem(ulong serverId) : base(serverId)
        {
        }
    }
}