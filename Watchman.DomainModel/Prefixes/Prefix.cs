using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Prefixes
{
    public class Prefix : Entity, IAggregateRoot
    {
        public ulong ServerId { get; private set; }
        public string Value { get; private set; }

        public Prefix(ulong serverId, string value)
        {
            this.ServerId = serverId;
            this.Value = value;
        }

        public void SetValue(string value)
        {
            if (this.Value == value)
            {
                return;
            }
            this.Value = value;
            this.Update();
        }
    }
}
