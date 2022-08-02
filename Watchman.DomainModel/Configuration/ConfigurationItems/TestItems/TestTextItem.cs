namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestTextItem : MappedConfiguration<string>
    {
        public override string Value { get; set; } = "default";
        public override string Group { get; set; } = "TestItems";

        public TestTextItem(ulong serverId) : base(serverId)
        {
        }
    }
}
