using System;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Configuration
{
    public class InitEvent : Entity
    {
        public ulong ServerId { get; private set; }
        public DateTime EndedAt { get; private set; }

        public InitEvent(ulong serverId, DateTime endedAt)
        {
            this.ServerId = serverId;
            this.EndedAt = endedAt;
        }
    }
}
