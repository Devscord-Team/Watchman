namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class PercentOfSimilarityBetweenMessagesToSuspectSpam : MappedConfiguration<double>
    {
        public override string ConfigurationName { get; } = nameof(PercentOfSimilarityBetweenMessagesToSuspectSpam);
        public override double DefaultValue { get; } = 0.4;
    }
}
