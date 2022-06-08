using System;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Configuration
{
    public class InitEvent : Entity
    {
        public ulong ServerId { get; private set; }
        public DateTime EndedAt { get; private set; }
        public int EndedDay { get; private set; }
        public int EndedHour { get; private set; }
        public int EndedMinute { get; private set; }

        public InitEvent(ulong serverId, DateTime endedAt)
        {
            this.ServerId = serverId;
            this.EndedAt = endedAt;
            this.EndedDay = endedAt.Day;
            this.EndedHour = endedAt.Hour;
            this.EndedMinute = endedAt.Minute;
        }
    }
}
