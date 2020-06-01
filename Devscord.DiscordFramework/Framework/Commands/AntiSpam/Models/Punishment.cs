using System;

namespace Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models
{
    public class Punishment
    {
        public PunishmentOption PunishmentOption { get; }
        public TimeSpan? ForTime { get; }

        public Punishment(PunishmentOption punishmentOption, TimeSpan? forTime = null)
        {
            PunishmentOption = punishmentOption;
            ForTime = forTime;
        }
    }
}
