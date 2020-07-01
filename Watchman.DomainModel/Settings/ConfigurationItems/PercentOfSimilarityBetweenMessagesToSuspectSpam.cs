namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class PercentOfSimilarityBetweenMessagesToSuspectSpam : MappedConfiguration<double>
    {
        public override string Name { get; } = nameof(PercentOfSimilarityBetweenMessagesToSuspectSpam);
        public override double DefaultValue { get; } = 0.4;
    }
}
