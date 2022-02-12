using System;

namespace Devscord.DiscordFramework.Commands.AntiSpam.Models
{
    public readonly struct Punishment
    {
        public PunishmentOption PunishmentOption { get; }
        public DateTime GivenAt { get; }
        public TimeSpan? ForTime { get; }

        public Punishment(PunishmentOption punishmentOption, DateTime givenAt, TimeSpan? forTime = null)
        {
            this.PunishmentOption = punishmentOption;
            this.GivenAt = givenAt;
            this.ForTime = forTime;
        }
    }
}
