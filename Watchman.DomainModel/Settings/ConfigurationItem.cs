using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings
{
    public class ConfigurationItem : Entity, IAggregateRoot
    {
        public object Value { get; private set; }
        public ulong ServerId { get; private set; }
        public string Name { get; private set; }

        public ConfigurationItem(object value, ulong serverId, string name)
        {
            Value = value;
            ServerId = serverId;
            Name = name;
        }

        public void SetValue(object value)
        {
            Value = value;
            Update();
        }
    }
}