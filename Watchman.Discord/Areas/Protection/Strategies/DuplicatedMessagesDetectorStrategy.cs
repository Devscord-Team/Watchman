using System;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class DuplicatedMessagesDetectorStrategy : ISpamDetector
    {
        public IUserMessagesCounter UserMessagesCounter { get; set; }

        public DuplicatedMessagesDetectorStrategy(IUserMessagesCounter userMessagesCounter)
        {
            UserMessagesCounter = userMessagesCounter;
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
            var similarMessagesCount = lastFewMessages.Count(x => GetDifferencePercent(x.Content, content) < 0.4);
            var serverId = contexts.Server.Id;
            
            var userCount = this.UserMessagesCounter.CountUserMessages(userId, serverId);
            var countToBeSafe = this.UserMessagesCounter.UserMessagesCountToBeSafe;

            return similarMessagesCount switch
            {
                0 => SpamProbability.None,
                1 => SpamProbability.Low,
                2 when userCount >= countToBeSafe => SpamProbability.Low,
                2 => SpamProbability.Medium,
                _ when userCount >= countToBeSafe => SpamProbability.Medium,
                _ => SpamProbability.Sure
            };
        }

        // copied from source: https://gist.github.com/Davidblkx/e12ab0bb2aff7fd8072632b396538560
        // modified: returns
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
