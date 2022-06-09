using System;
using System.Collections.Generic;
using System.Linq;
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
        public string NameCopy { get; private set; }

        public ConfigurationItem(object value, ulong serverId, string name, string group, string subGroup)
        {
            this.Value = value;
            this.ServerId = serverId;
            this.Name = name;
            this.Group = group;
            this.SubGroup = subGroup;
            this.NameCopy = name;
        }

        public void SetValue(object value)
        {
            if (this.Value == value)
            {
                return;
            }
            this.Value = value;
            this.Update();
        }

        public void SetGroup(string group)
        {
            if (this.Group == group)
            {
                return;
            }
            this.Group = group;
            this.Update();
        }

        public void SetSubGroup(string subGroup)
        {
            if (this.SubGroup == subGroup)
            {
                return;
            }
            this.SubGroup = subGroup;
            this.Update();
        }

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                throw new Exception("ConfigurationItem.Name must be not empty");
            }
            if (string.IsNullOrWhiteSpace(this.Group))
            {
                throw new Exception("ConfigurationItem.Group must be not empty");
            }
        }

        public bool Compare(ConfigurationItem item)
            => this.Name == item.Name
            && this.Group == item.Group
            && this.SubGroup == item.SubGroup;
    }
}