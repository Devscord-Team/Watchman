using System;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Configuration
{
    public class ConfigurationItem : Entity, IAggregateRoot
    {
        public object Value { get; private set; }
        public ulong ServerId { get; private set; }
        public string Name { get; private set; }
        public string Group { get; private set; }
        public string SubGroup { get; private set; }

        public ConfigurationItem(object value, ulong serverId, string name)
        {
            this.Value = value;
            this.ServerId = serverId;
            this.Name = name;
        }

        public void SetValue(object value)
        {
            this.Value = value;
            this.Update();
        }

        public override void Validate()
        {
            if (this.Value == null)
            {
                throw new Exception("ConfigurationItem.Value must be not null");
            }
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                throw new Exception("ConfigurationItem.Name must be not empty");
            }
            if (string.IsNullOrWhiteSpace(this.Group))
            {
                throw new Exception("ConfigurationItem.Group must be not empty");
            }
        }
    }
}