namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class PercentOfSimilarityBetweenMessagesToSuspectSpam : MappedConfiguration<double>
    {
        public override double DefaultValue { get; } = 0.4;
    }
}
