using System;

namespace Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models
{
    public class Punishment
    {
        public PunishmentOption PunishmentOption { get; }
        public DateTime GivenAt { get; }
        public TimeSpan? ForTime { get; }

        public Punishment(PunishmentOption punishmentOption, DateTime givenAt, TimeSpan? forTime = null)
        {
            PunishmentOption = punishmentOption;
            GivenAt = givenAt;
            ForTime = forTime;
        }
    }
}
