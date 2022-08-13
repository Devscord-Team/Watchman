namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestNullableFloatItem : MappedConfiguration<float?>
    {
        public override float? Value { get; set; } = 0;
        public override string Group { get; set; } = "TestItems";

        public TestNullableFloatItem(ulong serverId) : base(serverId)
        {
        }
    }
}
