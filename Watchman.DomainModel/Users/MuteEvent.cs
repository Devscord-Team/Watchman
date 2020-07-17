using Watchman.Common.Models;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Users
{
    public class MuteEvent : Entity
    {
        public ulong UserId { get; private set; }
        public string Reason { get; private set; }
        public TimeRange TimeRange { get; private set; }
        public ulong ServerId { get; private set; }
        public ulong MutedOnChannelId { get; private set; }
        public bool IsUnmuted { get; private set; }

        public MuteEvent(ulong userId, TimeRange timeRange, string reason, ulong serverId, ulong channelId)
        {
            this.UserId = userId;
            this.TimeRange = timeRange;
            this.Reason = reason;
            this.ServerId = serverId;
            this.MutedOnChannelId = channelId;
        }

        public void Unmute()
        {
            if(this.IsUnmuted)
            {
                return;
            }
            this.IsUnmuted = true;
            this.Update();
        }
    }
}
