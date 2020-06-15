using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings
{
    public class Configuration : Entity, IAggregateRoot
    {
        public int MinAverageMessagesPerWeek { get; private set; }

        internal static Configuration Default => new Configuration
        {
            MinAverageMessagesPerWeek = 20
        };
    }
}
