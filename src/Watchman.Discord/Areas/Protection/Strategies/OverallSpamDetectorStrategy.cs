using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.DomainModel.Settings.Services;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class OverallSpamDetectorStrategy : IOverallSpamDetector
    {
        private readonly ServerMessagesCacheService _serverMessagesCacheService;
        private readonly List<ISpamDetector> _spamDetectors;

        public static OverallSpamDetectorStrategy GetStrategyWithDefaultDetectors(ServerMessagesCacheService serverMessagesCacheService, IUserSafetyChecker userSafetyChecker, IConfigurationService configurationService)
        {
            return new OverallSpamDetectorStrategy(serverMessagesCacheService, new List<ISpamDetector>
            {
                new LinksDetectorStrategy(userSafetyChecker),
                new DuplicatedMessagesDetectorStrategy(userSafetyChecker, configurationService)
            });
        }

        public OverallSpamDetectorStrategy(ServerMessagesCacheService serverMessagesCacheService, List<ISpamDetector> spamDetectors)
        {
            this._serverMessagesCacheService = serverMessagesCacheService;
            this._spamDetectors = spamDetectors;
        }

        public SpamProbability GetOverallSpamProbability(DiscordRequest request, Contexts contexts)
        {
            var probabilities = this._spamDetectors
                .Select(x => x.GetSpamProbability(this._serverMessagesCacheService, request, contexts))
                .Where(x => x != SpamProbability.None)
                .ToList();

            if (probabilities.Count == 0)
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
