namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class MinAverageMessagesPerWeek : MappedConfiguration<int>
    {
        public override int DefaultValue { get; } = 20;
    }
}
