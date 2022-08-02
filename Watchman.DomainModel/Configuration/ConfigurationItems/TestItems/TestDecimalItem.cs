namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestDecimalItem : MappedConfiguration<decimal>
    {
        public override decimal Value { get; set; } = 0;
        public override string Group { get; set; } = "TestItems";

        public TestDecimalItem(ulong serverId) : base(serverId)
        {
        }
    }
}
