using System;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Antispam
{
    public class ProtectionPunishment : Entity, IAggregateRoot
    {
        public ProtectionPunishmentOption Option { get; private set; }
        public ulong UserId { get; private set; }
        public DateTime GivenAt { get; private set; }
        public TimeSpan? Time { get; private set; }

        public ProtectionPunishment(ProtectionPunishmentOption option, ulong userId, DateTime givenAt, TimeSpan? time = null)
        {
            this.Option = option;
            this.UserId = userId;
            this.GivenAt = givenAt;
            this.Time = time;
        }
    }
}