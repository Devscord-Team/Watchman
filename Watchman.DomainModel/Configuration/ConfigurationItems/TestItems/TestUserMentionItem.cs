namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestUserMentionItem : MappedConfiguration<ulong>
    {
        public override ulong Value { get; set; } = 0;
        public override string Group { get; set; } = "TestItems";

        public TestUserMentionItem(ulong serverId) : base(serverId)
        {
        }
    }
}
