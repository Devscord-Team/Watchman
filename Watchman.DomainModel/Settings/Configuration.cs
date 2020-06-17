using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings
{
    public class Configuration : Entity, IAggregateRoot
    {
        public int MinAverageMessagesPerWeek { get; private set; }
        public double PercentOfSimilarityBetweenMessagesToSuspectSpam { get; private set; }

        public static Configuration Default => new Configuration
        {
            MinAverageMessagesPerWeek = 20,
            PercentOfSimilarityBetweenMessagesToSuspectSpam = 0.4
        };
    }
}
