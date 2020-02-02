using Watchman.Common.Models;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Mute
{
    public class MuteEvent : Entity
    {
        public ulong UserId { get; private set; }
        public string Reason { get; private set; }
        public TimeRange TimeRange { get; private set; }

        public MuteEvent(ulong userId, TimeRange timeRange, string reason)
        {
            UserId = userId;
            TimeRange = timeRange;
            Reason = reason;
        }
    }
}
