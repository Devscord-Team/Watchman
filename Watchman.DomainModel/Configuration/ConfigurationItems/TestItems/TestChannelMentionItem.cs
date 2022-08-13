namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestChannelMentionItem : MappedConfiguration<ulong>
    {
        public override ulong Value { get; set; } = 0;
        public override string Group { get; set; } = "TestItems";

        public TestChannelMentionItem(ulong serverId) : base(serverId)
        {
        }
    }
}
