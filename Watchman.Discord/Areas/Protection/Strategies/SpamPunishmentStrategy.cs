using System;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Watchman.Discord.Areas.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class SpamPunishmentStrategy : ISpamPunishmentStrategy
    {
        private readonly IPunishmentsCachingService _punishmentsCachingService;

        public SpamPunishmentStrategy(IPunishmentsCachingService punishmentsCachingService)
        {
            _punishmentsCachingService = punishmentsCachingService;
        }

        public Punishment GetPunishment(ulong userId, SpamProbability spamProbability)
        {
            var takeFromTime = DateTime.Now.AddHours(-12);
            var warnsCount = _punishmentsCachingService.GetUserWarnsCount(userId, takeFromTime);

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
                var mutesCount = _punishmentsCachingService.GetUserMutesCount(userId, takeFromTime);
                return new Punishment(PunishmentOption.Mute, DateTime.Now, GetTimeForMute(mutesCount));
            }
            return new Punishment(punishmentOption, DateTime.Now);
        }

        private TimeSpan GetTimeForMute(int userMutesCount)
        {
            const int maxMinutesValue = int.MaxValue / 60 / 1000; // milliseconds must be less than int.MaxValue
            var minutes = 15 * Math.Pow(2, userMutesCount);
            if (minutes > maxMinutesValue)
            {
                return TimeSpan.FromMinutes(maxMinutesValue);
            }
            return TimeSpan.FromMinutes(minutes);
        }
    }
}
