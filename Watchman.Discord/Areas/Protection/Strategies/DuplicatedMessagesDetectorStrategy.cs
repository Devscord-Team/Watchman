using System;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.DomainModel.Settings.ConfigurationItems;
using Watchman.DomainModel.Settings.Services;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class DuplicatedMessagesDetectorStrategy : ISpamDetector
    {
        public IUserSafetyChecker UserSafetyChecker { get; set; }

        private readonly IConfigurationService _configurationService;

        public DuplicatedMessagesDetectorStrategy(IUserSafetyChecker userSafetyChecker, IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
            this.UserSafetyChecker = userSafetyChecker;
        }

        public SpamProbability GetSpamProbability(ServerMessagesCacheService serverMessagesCacheService, DiscordRequest request, Contexts contexts)
        {
            var userId = contexts.User.Id;
            var lastFewMessages = serverMessagesCacheService.GetLastUserMessages(userId)
                .TakeLast(4)
                .ToList();

            if (lastFewMessages.Count < 2)
            {
                return SpamProbability.None;
            }

            var content = request.OriginalMessage;
            var serverId = contexts.Server.Id;
            var percentOfSimilarityToSuspectSpam = this._configurationService.GetConfigurationItem<PercentOfSimilarityBetweenMessagesToSuspectSpam>(serverId).Value;
            var similarMessagesCount = lastFewMessages.Count(x => GetDifferencePercent(x.Content, content) < percentOfSimilarityToSuspectSpam);
            var isUserSafe = this.UserSafetyChecker.IsUserSafe(userId, serverId);

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
                return 0;
            
            var source1Length = message1.Length;
            var source2Length = message2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
                return 1;

            if (source2Length == 0)
                return 1;

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (var i = 0; i <= source1Length; matrix[i, 0] = i++){}
            for (var j = 0; j <= source2Length; matrix[0, j] = j++){}

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
            return (double)matrix[source1Length, source2Length] / source1Length;
        }
    }
}
