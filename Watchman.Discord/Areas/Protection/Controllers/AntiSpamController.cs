using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Protection.Strategies;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AntiSpamController : IController
    {
        private readonly ServerMessagesCacheService _serverMessagesCacheService;
        private readonly PunishmentsCachingService _punishmentsCachingService;
        private readonly AntiSpamService _antiSpamService;
        private readonly IOverallSpamDetector _overallSpamDetector;
        private readonly ISpamPunishmentStrategy _spamPunishmentStrategy;

        public AntiSpamController(ServerMessagesCacheService serverMessagesCacheService, UserMessagesCountService userMessagesCounterService, PunishmentsCachingService punishmentsCachingService, AntiSpamService antiSpamService)
        {
            _serverMessagesCacheService = serverMessagesCacheService;
            _punishmentsCachingService = punishmentsCachingService;
            _antiSpamService = antiSpamService;
            _overallSpamDetector = OverallSpamDetectorStrategy.GetStrategyWithDefaultDetectors(serverMessagesCacheService, userMessagesCounterService);
            _spamPunishmentStrategy = new SpamPunishmentStrategy(punishmentsCachingService);
        }

        [ReadAlways]
        public async Task Scan(DiscordRequest request, Contexts contexts)
        {
            Log.Information("Started scanning the message");

            var spamProbability = _overallSpamDetector.GetOverallSpamProbability(request, contexts);
            if (spamProbability != SpamProbability.None)
            {
                var punishment = _spamPunishmentStrategy.GetPunishment(contexts.User.Id, spamProbability);
                await _antiSpamService.SetPunishment(contexts, punishment);
                await _punishmentsCachingService.AddUserPunishment(contexts.User.Id, punishment);
            }
            _serverMessagesCacheService.AddMessage(request, contexts);
            Log.Information("Scanned");
        }
    }
}
