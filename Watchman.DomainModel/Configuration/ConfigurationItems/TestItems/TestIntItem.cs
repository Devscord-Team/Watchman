namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestIntItem : MappedConfiguration<int>
    {
        public override int Value { get; set; } = 0;
        public override string Group { get; set; } = "TestItems";

        public TestIntItem(ulong serverId) : base(serverId)
        {
        }
    }
}
