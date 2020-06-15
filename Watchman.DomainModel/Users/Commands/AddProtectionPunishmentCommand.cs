using System;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Users.Commands
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
