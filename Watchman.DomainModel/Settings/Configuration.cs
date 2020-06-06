using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings
{
    public class Configuration : Entity, IAggregateRoot
    {
        public int UserMessagesCountToBeSafe { get; private set; }

        internal static Configuration GetDefaultConfiguration()
        {
            return new Configuration
            {
                UserMessagesCountToBeSafe = 500
            };
        }
    }
}
