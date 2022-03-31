using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Commands.AntiSpam;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;

namespace Watchman.Discord.Areas.AntiSpam.Strategies
{
    public class OverallSpamDetectorStrategy : IOverallSpamDetector
    {
        private readonly IServerMessagesCacheService _serverMessagesCacheService;
        private readonly List<ISpamDetector> _spamDetectors;

        public OverallSpamDetectorStrategy(IServerMessagesCacheService serverMessagesCacheService, List<ISpamDetector> spamDetectors)
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
