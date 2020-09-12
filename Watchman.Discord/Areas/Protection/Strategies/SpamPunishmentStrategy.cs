using System;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Serilog;
using Watchman.Discord.Areas.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class SpamPunishmentStrategy : ISpamPunishmentStrategy
    {
        private readonly IPunishmentsCachingService _punishmentsCachingService;
        private readonly WarnsService _warnsService;

        public SpamPunishmentStrategy(IPunishmentsCachingService punishmentsCachingService, WarnsService warnsService)
        {
            this._punishmentsCachingService = punishmentsCachingService;
            this._warnsService = warnsService;
        }

        public Punishment GetPunishment(ulong userId, ulong serverId, SpamProbability spamProbability)
        {
            var takeFromTime = DateTime.Now.AddHours(-12);
            var warnsCount = _warnsService.GetWarnsCount(userId, serverId, takeFromTime);              
            Log.Information("User {userId} has {warnsCount} warns", userId, warnsCount);
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
                var mutesCount = this._punishmentsCachingService.GetUserMutesCount(userId, takeFromTime);
                return new Punishment(PunishmentOption.Mute, DateTime.UtcNow, this.GetTimeForMute(mutesCount));
            }
            return new Punishment(punishmentOption, DateTime.UtcNow);
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
