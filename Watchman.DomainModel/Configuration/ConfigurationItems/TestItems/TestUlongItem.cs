namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestUlongItem : MappedConfiguration<ulong>
    {
        public override ulong Value { get; set; } = 0;
        public override string Group { get; set; } = "TestItems";

        public TestUlongItem(ulong serverId) : base(serverId)
        {
        }
    }
}
