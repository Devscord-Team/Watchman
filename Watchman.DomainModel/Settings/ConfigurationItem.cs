using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings
{
    public class ConfigurationItem : Entity, IAggregateRoot
    {
        public object Value { get; set; }
        public ulong ServerId { get; set; }
        public string ConfigurationName { get; set; }
    }
}
