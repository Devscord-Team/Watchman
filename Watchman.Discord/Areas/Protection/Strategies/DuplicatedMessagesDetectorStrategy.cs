using System;
using System.Linq;
using Devscord.DiscordFramework.Commands.AntiSpam;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.DomainModel.Configuration.ConfigurationItems;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class DuplicatedMessagesDetectorStrategy : ISpamDetector
    {
        private readonly IUserSafetyChecker _userSafetyChecker;
        private readonly IConfigurationService _configurationService;

        public DuplicatedMessagesDetectorStrategy(IUserSafetyChecker userSafetyChecker, IConfigurationService configurationService)
        {
            this._userSafetyChecker = userSafetyChecker;
            this._configurationService = configurationService;
        }

        public SpamProbability GetSpamProbability(ServerMessagesCacheService serverMessagesCacheService, Contexts contexts)
        {
            var userId = contexts.User.Id;
            var serverId = contexts.Server.Id;
            var lastFewMessages = serverMessagesCacheService.GetLastUserMessages(userId, serverId)
                .TakeLast(5)
                .ToList();

            if (lastFewMessages.Count < 2)
            {
                return SpamProbability.None;
            }
            var percentOfSimilarityToSuspectSpam = this._configurationService.GetConfigurationItem<PercentOfSimilarityBetweenMessagesToSuspectSpam>(serverId).Value;
            var content = lastFewMessages.Last().Content;
            var similarMessagesCount = lastFewMessages
                .SkipLast(1) // because I'm comparing all the other messages to the last message
                .Count(x => this.GetDifferencePercent(x.Content, content) < percentOfSimilarityToSuspectSpam);

            var isUserSafe = this._userSafetyChecker.IsUserSafe(userId, serverId);
            return similarMessagesCount switch
            {
                0 => SpamProbability.None,
                1 => SpamProbability.Low,
                2 when isUserSafe => SpamProbability.Low,
                2 => SpamProbability.Medium,
                _ when isUserSafe => SpamProbability.Medium,
                _ => SpamProbability.Sure
            };
        }

        private double GetDifferencePercent(string message1, string message2)
        {
            if (message1 == message2)
            {
                return 0;
            }

            var source1Length = message1.Length;
            var source2Length = message2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
            {
                return 1;
            }

            if (source2Length == 0)
            {
                return 1;
            }

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (var i = 0; i <= source1Length; matrix[i, 0] = i++)
            { }
            for (var j = 0; j <= source2Length; matrix[0, j] = j++)
            { }

            // Calculate rows and columns distances
            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = message2[j - 1] == message1[i - 1] ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            // return result
            return (double) matrix[source1Length, source2Length] / source1Length;
        }
    }
}
