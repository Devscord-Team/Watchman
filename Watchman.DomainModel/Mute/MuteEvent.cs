using Watchman.Common.Models;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Mute
{
    public class MuteEvent : Entity
    {
        public ulong UserId { get; }
        public TimeRange TimeRange { get; }
        public string Reason { get; }

        public MuteEvent(ulong userId, TimeRange timeRange, string reason)
        {
            UserId = userId;
            TimeRange = timeRange;
            Reason = reason;
        }
    }
}
