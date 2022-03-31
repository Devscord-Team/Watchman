using System;
using Watchman.Cqrs;
using Watchman.DomainModel.Antispam;

namespace Watchman.DomainModel.Antispam.Commands
{
    public class AddProtectionPunishmentCommand : ICommand
    {
        public ProtectionPunishment ProtectionPunishment { get; }

        public AddProtectionPunishmentCommand(ProtectionPunishmentOption protectionPunishmentOption, ulong userId, DateTime givenAt, TimeSpan? forTime = null)
        {
            this.ProtectionPunishment = new ProtectionPunishment(protectionPunishmentOption, userId, givenAt, forTime);
        }

        public AddProtectionPunishmentCommand(ProtectionPunishment protectionPunishment)
        {
            this.ProtectionPunishment = protectionPunishment;
        }
    }
}
