namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestFloatItem : MappedConfiguration<float>
    {
        public override float Value { get; set; } = 0;
        public override string Group { get; set; } = "TestItems";

        public TestFloatItem(ulong serverId) : base(serverId)
        {
        }
    }
}
