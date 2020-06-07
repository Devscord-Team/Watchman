﻿using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class OverallSpamDetectorStrategy : IOverallSpamDetector
    {
        private readonly ServerMessagesCacheService _serverMessagesCacheService;
        private readonly List<ISpamDetector> _spamDetectors;

        public static OverallSpamDetectorStrategy GetStrategyWithDefaultDetectors(ServerMessagesCacheService serverMessagesCacheService, IUserMessagesCounter userMessagesCounter)
        {
            return new OverallSpamDetectorStrategy(serverMessagesCacheService, new List<ISpamDetector>
            {
                new LinksDetectorStrategy(userMessagesCounter),
                new DuplicatedMessagesDetectorStrategy(userMessagesCounter)
            });
        }

        public OverallSpamDetectorStrategy(ServerMessagesCacheService serverMessagesCacheService, List<ISpamDetector> spamDetectors)
        {
            this._serverMessagesCacheService = serverMessagesCacheService;
            this._spamDetectors = spamDetectors;
        }

        public SpamProbability GetOverallSpamProbability(Message message)
        {
            var probabilities = _spamDetectors
                .Select(x => x.GetSpamProbability(_serverMessagesCacheService, message))
                .Where(x => x > SpamProbability.None)
                .ToList();

            if (probabilities.Count == 0)
            {
                return SpamProbability.None;
            }

            if (probabilities.Contains(SpamProbability.Sure))
            {
                return SpamProbability.Sure;
            }

            if (probabilities.Count(x => x == SpamProbability.Medium) >= 2)
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