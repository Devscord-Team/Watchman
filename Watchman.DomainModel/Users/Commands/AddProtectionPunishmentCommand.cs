using System;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Users.Commands
{
    public class AddProtectionPunishmentCommand : ICommand
    {
        public ProtectionPunishment ProtectionPunishment { get; set; }

        public AddProtectionPunishmentCommand(ProtectionPunishmentOption protectionPunishmentOption, ulong userId, TimeSpan? forTime = null)
        {
            this.ProtectionPunishment = new ProtectionPunishment(protectionPunishmentOption, userId, forTime);
        }

        public AddProtectionPunishmentCommand(ProtectionPunishment protectionPunishment)
        {
            this.ProtectionPunishment = protectionPunishment;
        }
    }
}
