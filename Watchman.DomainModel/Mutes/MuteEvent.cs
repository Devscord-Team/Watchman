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
        public bool Unmuted
        {
            get => this._unmuted;
            set
            {
                this._unmuted = value;
                this.Update();
            }
        }

        private bool _unmuted;

        public MuteEvent(ulong userId, TimeRange timeRange, string reason, ulong serverId)
        {
            this.UserId = userId;
            this.TimeRange = timeRange;
            this.Reason = reason;
            this.ServerId = serverId;
        }
    }
}
