using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.DomainModel.Settings.Services;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AntiSpamController : IController
    {
        private readonly ServerMessagesCacheService _serverMessagesCacheService;
        private readonly PunishmentsCachingService _punishmentsCachingService;
        private readonly AntiSpamService _antiSpamService;
        private readonly IOverallSpamDetector _overallSpamDetector;
        private readonly ISpamPunishmentStrategy _spamPunishmentStrategy;

        // to avoid giving a few mutes in just a few seconds to the same user
        private static readonly Dictionary<ulong, DateTime> _lastUserPunishmentDate = new Dictionary<ulong, DateTime>();
        // it's really needed - to avoid multiple warning and muting the same user
        private static bool _isNowChecking;

        public AntiSpamController(ServerMessagesCacheService serverMessagesCacheService, CheckUserSafetyService checkUserSafetyService, PunishmentsCachingService punishmentsCachingService, AntiSpamService antiSpamService, ConfigurationService configurationService)
        {
            this._serverMessagesCacheService = serverMessagesCacheService;
            this._punishmentsCachingService = punishmentsCachingService;
            this._antiSpamService = antiSpamService;
            this._overallSpamDetector = OverallSpamDetectorStrategy.GetStrategyWithDefaultDetectors(serverMessagesCacheService, checkUserSafetyService, configurationService);
            this._spamPunishmentStrategy = new SpamPunishmentStrategy(punishmentsCachingService);
        }

        [ReadAlways]
        public async Task Scan(DiscordRequest request, Contexts contexts)
        {
            var stopwatch = Stopwatch.StartNew();
            Log.Information("Started scanning the message");

            if (this.ShouldCheckThisMessage(contexts.User.Id, request))
            {
                _isNowChecking = true;
                var spamProbability = this._overallSpamDetector.GetOverallSpamProbability(request, contexts);
                if (spamProbability != SpamProbability.None)
                {
                    Log.Information("{SpamProbability} for {user}", spamProbability, contexts.User.Name);
                    await this.HandlePossibleSpam(contexts, spamProbability, request.SentAt);
                }
                _isNowChecking = false;
            }
            this._serverMessagesCacheService.AddMessage(request, contexts);
            Log.Information("Scanned");
            Log.Information("antispam: {ticks}ticks", stopwatch.ElapsedTicks);
        }

        private bool ShouldCheckThisMessage(ulong userId, DiscordRequest request)
        {
            if (_isNowChecking || request.IsCommandForBot) // interactions with bot shouldn't be considered as spam
            {
                return false;
            }
            return !_lastUserPunishmentDate.TryGetValue(userId, out var time) || time < request.SentAt.AddSeconds(-5);
        }

        private async Task HandlePossibleSpam(Contexts contexts, SpamProbability spamProbability, DateTime messageSentAt)
        {
            var punishment = this._spamPunishmentStrategy.GetPunishment(contexts.User.Id, spamProbability);
            await this._antiSpamService.SetPunishment(contexts, punishment);
            await this._punishmentsCachingService.AddUserPunishment(contexts.User.Id, punishment);

            if (punishment.PunishmentOption != PunishmentOption.Nothing)
            {
                this.UpdateLastPunishmentDate(contexts.User.Id, messageSentAt);
                Log.Information("{PunishmentOption} for user: {user}", punishment.PunishmentOption, contexts.User.Name);
            }
        }

        private void UpdateLastPunishmentDate(ulong userId, DateTime messageSentAt)
        {
            if (!_lastUserPunishmentDate.TryAdd(userId, messageSentAt))
            {
                _lastUserPunishmentDate[userId] = messageSentAt;
            }
        }
    }
}
