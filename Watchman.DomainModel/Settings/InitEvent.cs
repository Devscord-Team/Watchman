using System;

using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Settings
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
