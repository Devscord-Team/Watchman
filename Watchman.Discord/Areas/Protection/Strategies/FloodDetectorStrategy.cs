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
    public class FloodDetectorStrategy : ISpamDetector
    {
        private readonly IConfigurationService _configurationService;
        public IUserSafetyChecker UserSafetyChecker { get; set; }

        public FloodDetectorStrategy(IUserSafetyChecker userSafetyChecker, IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
            this.UserSafetyChecker = userSafetyChecker;
        }

        public SpamProbability GetSpamProbability(ServerMessagesCacheService serverMessagesCacheService, DiscordRequest request, Contexts contexts)
        {
            var howManyMessagesCount = this._configurationService.GetConfigurationItem<HowManyMessagesInShortTimeToBeSpam>(contexts.Server.Id).Value;
            var howManySeconds = this._configurationService.GetConfigurationItem<HowLongIsShortTimeInSeconds>(contexts.Server.Id).Value;
            var minDate = DateTime.UtcNow.AddSeconds(-howManySeconds);
            var messagesCount = serverMessagesCacheService.GetLastUserMessages(contexts.User.Id, contexts.Server.Id)
                .TakeLast(howManyMessagesCount)
                .Count(x => x.SentAt >= minDate) + 1; // +1 bcs the message which is handled now is not contained in serverMessagesCacheService

            if (messagesCount <= howManyMessagesCount / 3)
            {
                return SpamProbability.None;
            }
            var userIsSafe = this.UserSafetyChecker.IsUserSafe(contexts.User.Id, contexts.Server.Id);
            return userIsSafe switch //todo: rewrite this switch when we'll switch to c# 9
            {
                true when messagesCount < howManyMessagesCount => SpamProbability.None,
                false when messagesCount <= howManyMessagesCount / 2 => SpamProbability.Low,
                false when messagesCount < howManyMessagesCount => SpamProbability.Medium,
                _ => SpamProbability.Sure
            };
        }
    }
}
