using System;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Watchman.Discord.Areas.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class SpamPunishmentStrategy : ISpamPunishmentStrategy
    {
        private readonly PunishmentsCachingService _punishmentsCachingService;

        public SpamPunishmentStrategy(PunishmentsCachingService punishmentsCachingService)
        {
            _punishmentsCachingService = punishmentsCachingService;
        }

        public Punishment GetPunishment(ulong userId, SpamProbability spamProbability)
        {
            var userPunishments = _punishmentsCachingService.GetUserPunishments(userId);
            var startTime = DateTime.Now.AddHours(-12);
            var warnsCount = userPunishments.Count(x => x.PunishmentOption == PunishmentOption.Warn && x.GivenAt > startTime);

            var punishmentOption = spamProbability switch
            {
                SpamProbability.None => PunishmentOption.Nothing,
                SpamProbability.Low => PunishmentOption.Nothing,
                SpamProbability.Medium when warnsCount == 0 => PunishmentOption.Nothing,

                SpamProbability.Medium when warnsCount <= 2 => PunishmentOption.Warn,
                SpamProbability.Sure when warnsCount == 0 => PunishmentOption.Warn,

                SpamProbability.Medium when warnsCount > 2 => PunishmentOption.Mute,
                SpamProbability.Sure when warnsCount >= 1 => PunishmentOption.Mute,
                _ => throw new NotImplementedException()
            };

            if (punishmentOption == PunishmentOption.Mute)
            {
                var mutesCount = userPunishments.Count(x => x.PunishmentOption == PunishmentOption.Mute && x.GivenAt > startTime);
                return new Punishment(PunishmentOption.Mute, DateTime.Now, GetTimeForMute(mutesCount));
            }
            return new Punishment(punishmentOption, DateTime.Now);
        }

        private TimeSpan GetTimeForMute(int userMutesCount)
        {
            var minutes = 15 * Math.Pow(2, userMutesCount);
            return TimeSpan.FromMinutes(minutes);
        }
    }
}
