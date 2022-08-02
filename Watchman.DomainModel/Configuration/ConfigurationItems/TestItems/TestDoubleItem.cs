namespace Watchman.DomainModel.Configuration.ConfigurationItems.TestItems
{
    public class TestDoubleItem : MappedConfiguration<double>
    {
        public override double Value { get; set; } = 0;
        public override string Group { get; set; } = "TestItems";

        public TestDoubleItem(ulong serverId) : base(serverId)
        {
        } 
    }
}
