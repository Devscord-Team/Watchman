using System;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Users
{
    public class ProtectionPunishment : Entity, IAggregateRoot
    {
        public ProtectionPunishmentOption Option { get; private set; }
        public TimeSpan? Time { get; private set; }
        public ulong UserId { get; private set; }

        public ProtectionPunishment(ProtectionPunishmentOption option, ulong userId, TimeSpan? time = null)
        {
            this.Option = option;
            this.UserId = userId;
            this.Time = time;
        }
    }
}