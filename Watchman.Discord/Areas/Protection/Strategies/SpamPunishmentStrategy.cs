using System;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class SpamPunishmentStrategy : ISpamPunishmentStrategy
    {
        public Punishment GetPunishment(ulong userId, SpamProbability spamProbability)
        {
            return spamProbability switch
            { //todo: poprawić o korzystanie z domeny
                SpamProbability.None => new Punishment(PunishmentOption.Nothing),
                SpamProbability.Low => new Punishment(PunishmentOption.Nothing),
                SpamProbability.Medium => new Punishment(PunishmentOption.Warn),
                SpamProbability.Sure => new Punishment(PunishmentOption.Mute),
                _ => throw new NotImplementedException()
            };
        }
    }
}
