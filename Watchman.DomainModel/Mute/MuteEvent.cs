using Watchman.Common.Models;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Mute
{
    public class MuteEvent : Entity
    {
        public ulong UserId { get; private set; }
        public string Reason { get; private set; }
        public TimeRange TimeRange { get; private set; }
        public ulong ServerId { get; private set; }
        public bool Unmuted { get; private set; } = false;

        public MuteEvent(ulong userId, TimeRange timeRange, string reason, ulong serverId)
        {
            UserId = userId;
            TimeRange = timeRange;
            Reason = reason;
            ServerId = serverId;
        }
    }
}
