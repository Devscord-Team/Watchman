namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class MinAverageMessagesPerWeek : MappedConfiguration<int>
    {
        public override string Name { get; } = nameof(MinAverageMessagesPerWeek);
        public override int DefaultValue { get; } = 20;
    }
}
