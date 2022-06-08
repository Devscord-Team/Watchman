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
        public List<string> Groups { get; private set; }
        public string SubGroup { get; private set; }
        public string NameCopy { get; private set; }

        public ConfigurationItem(object value, ulong serverId, string name, List<string> groups, string subGroup)
        {
            this.Value = value;
            this.ServerId = serverId;
            this.Name = name;
            this.Groups = groups;
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
            if (this.Groups.FirstOrDefault() == group)
            {
                return;
            }
            this.Groups = new List<string> { group };
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
            if (string.IsNullOrWhiteSpace(this.Groups.FirstOrDefault()))
            {
                throw new Exception("ConfigurationItem.Groups must be not empty");
            }
        }

        public bool Compare(ConfigurationItem item)
            => this.Name == item.Name
            && this.Groups == item.Groups
            && this.SubGroup == item.SubGroup;
    }
}