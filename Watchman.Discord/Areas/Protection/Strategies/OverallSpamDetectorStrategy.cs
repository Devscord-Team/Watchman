using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Commands.AntiSpam;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class OverallSpamDetectorStrategy : IOverallSpamDetector
    {
        private readonly ServerMessagesCacheService _serverMessagesCacheService;
        private readonly List<ISpamDetector> _spamDetectors;

        //todo another class and singleton factory
        public static OverallSpamDetectorStrategy GetStrategyWithDefaultDetectors(ServerMessagesCacheService serverMessagesCacheService, IUserSafetyChecker userSafetyChecker, IConfigurationService configurationService)
        {
            return new OverallSpamDetectorStrategy(serverMessagesCacheService, new List<ISpamDetector>
            {
                new LinksDetectorStrategy(userSafetyChecker),
                new DuplicatedMessagesDetectorStrategy(userSafetyChecker, configurationService),
                new CapslockDetectorStrategy(userSafetyChecker, configurationService),
                new FloodDetectorStrategy(userSafetyChecker, configurationService)
            });
        }

        public OverallSpamDetectorStrategy(ServerMessagesCacheService serverMessagesCacheService, List<ISpamDetector> spamDetectors)
        {
            this._serverMessagesCacheService = serverMessagesCacheService;
            this._spamDetectors = spamDetectors;
        }

        public SpamProbability GetOverallSpamProbability(Contexts contexts)
        {
            var probabilities = this._spamDetectors
                .Select(x =>
                {
                    var spamProbability = x.GetSpamProbability(this._serverMessagesCacheService, contexts);
                    if (spamProbability != SpamProbability.None)
                    {
                        Log.Information("{detector} detected {spamProbability} spam probability", x.GetType().Name, spamProbability);
                    }
                    return spamProbability;
                })
                .Where(x => x != SpamProbability.None)
                .ToList();

            if (!probabilities.Any())
            {
                return SpamProbability.None;
            }
            if (probabilities.Contains(SpamProbability.Sure))
            {
                return SpamProbability.Sure;
            }
            if (probabilities.Count(x => x == SpamProbability.Medium) > 1)
            {
                return SpamProbability.Sure;
            }
            if (probabilities.Contains(SpamProbability.Medium))
            {
                return SpamProbability.Medium;
            }
            return SpamProbability.Low;
        }
    }
}
