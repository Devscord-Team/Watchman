namespace Watchman.DomainModel.Configuration.ConfigurationItems.AntiSpam
{
    public class PercentOfSimilarityBetweenMessagesToSuspectSpam : MappedConfiguration<double>
    {
        public override double Value { get; set; } = 0.4;
        public override string Group { get; set; } = "AntiSpam";

        public PercentOfSimilarityBetweenMessagesToSuspectSpam(ulong serverId) : base(serverId)
        {
        }
    }
}
