namespace Watchman.DomainModel.Settings.ConfigurationItems
{
    public class PercentOfSimilarityBetweenMessagesToSuspectSpam : MappedConfiguration<double>
    {
        public override double Value { get; set; } = 0.4;

        public PercentOfSimilarityBetweenMessagesToSuspectSpam(ulong serverId) : base(serverId)
        {
        }
    }
}
