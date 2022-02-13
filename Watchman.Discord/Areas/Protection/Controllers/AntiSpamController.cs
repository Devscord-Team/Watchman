using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands.AntiSpam;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AntiSpamController : IController
    {
        private readonly IServerMessagesCacheService _serverMessagesCacheService;
        private readonly IPunishmentsCachingService _punishmentsCachingService;
        private readonly IAntiSpamService _antiSpamService;
        private readonly IOverallSpamDetector _overallSpamDetector;
        private readonly ISpamPunishmentStrategy _spamPunishmentStrategy;

        // to avoid giving a few mutes in just a few seconds to the same user
        //changed to public because of tests
        public static readonly Dictionary<ulong, DateTime> LastUserPunishmentDate = new Dictionary<ulong, DateTime>(); 
        // it's really needed - to avoid multiple warning and muting the same user
        private static readonly List<ulong> usersNowChecking = new List<ulong>();

        public AntiSpamController(IServerMessagesCacheService serverMessagesCacheService, ICheckUserSafetyService checkUserSafetyService, IPunishmentsCachingService punishmentsCachingService, 
            IAntiSpamService antiSpamService, IConfigurationService configurationService, ISpamPunishmentStrategy spamPunishmentStrategy, IOverallSpamDetectorStrategyFactory overallSpamDetectorStrategyFactory)
        {
            this._serverMessagesCacheService = serverMessagesCacheService;
            this._punishmentsCachingService = punishmentsCachingService;
            this._antiSpamService = antiSpamService;
            this._overallSpamDetector = overallSpamDetectorStrategyFactory.GetStrategyWithDefaultDetectors(serverMessagesCacheService, checkUserSafetyService, configurationService);
            this._spamPunishmentStrategy = spamPunishmentStrategy;
        }

        [ReadAlways]
        public async Task Scan(DiscordRequest request, Contexts contexts)
        {
            this._serverMessagesCacheService.AddMessage(request, contexts);
            if (this.ShouldCheckThisMessage(contexts.User.Id, request) == false)
            {
                return;
            }
            usersNowChecking.Add(contexts.User.Id);
            var spamProbability = this._overallSpamDetector.GetOverallSpamProbability(contexts);
            if (spamProbability != SpamProbability.None)
            {
                await this.HandlePossibleSpam(contexts, spamProbability, request.SentAt);
            }
            usersNowChecking.Remove(contexts.User.Id);
        }

        private bool ShouldCheckThisMessage(ulong userId, DiscordRequest request)
            => usersNowChecking.Contains(userId) ? false 
            : !LastUserPunishmentDate.TryGetValue(userId, out var time) || time < request.SentAt.AddSeconds(-5);

        private async Task HandlePossibleSpam(Contexts contexts, SpamProbability spamProbability, DateTime messageSentAt)
        {
            var punishment = this._spamPunishmentStrategy.GetPunishment(contexts.User.Id, spamProbability);
            await this._antiSpamService.SetPunishment(contexts, punishment);

            if (punishment.PunishmentOption != PunishmentOption.Nothing)
            {
                await this._punishmentsCachingService.AddUserPunishment(contexts.User.Id, punishment);
                this.UpdateLastPunishmentDate(contexts.User.Id, messageSentAt);
            }
        }

        private void UpdateLastPunishmentDate(ulong userId, DateTime messageSentAt)
        {
            if (!LastUserPunishmentDate.TryAdd(userId, messageSentAt))
            {
                LastUserPunishmentDate[userId] = messageSentAt;
            }
        }
    }
}
