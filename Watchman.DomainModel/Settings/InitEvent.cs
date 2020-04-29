using System;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings
{
    public class InitEvent : Entity
    {
        public ulong ServerId { get; private set; }
        public DateTime EndedAt { get; private set; }

        public InitEvent(ulong serverId, DateTime endedAt)
        {
            ServerId = serverId;
            EndedAt = endedAt;
        }
    }
}
