using System;
using System.Linq;
using Devscord.DiscordFramework.Commands.AntiSpam;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.DomainModel.Configuration.ConfigurationItems;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class FloodDetectorStrategy : ISpamDetector
    {
        private readonly IConfigurationService _configurationService;
        //private readonly IUserSafetyChecker _userSafetyChecker;

        public FloodDetectorStrategy(/*IUserSafetyChecker userSafetyChecker,*/ IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
            //this._userSafetyChecker = userSafetyChecker;
        }

        public SpamProbability GetSpamProbability(IServerMessagesCacheService serverMessagesCacheService, Contexts contexts)
        {
            var howManyMessagesCount = this._configurationService.GetConfigurationItem<HowManyMessagesInShortTimeToBeSpam>(contexts.Server.Id).Value;
            var howManySeconds = this._configurationService.GetConfigurationItem<HowLongIsShortTimeInSeconds>(contexts.Server.Id).Value;
            var minDate = DateTime.UtcNow.AddSeconds(-howManySeconds);
            var messagesCount = serverMessagesCacheService.GetLastUserMessages(contexts.User.Id, contexts.Server.Id)
                .TakeLast(howManyMessagesCount)
                .Count(x => x.SentAt >= minDate);

            if (messagesCount <= howManyMessagesCount / 3)
            {
                return SpamProbability.None;
            }
            var userIsSafe = true; //this._userSafetyChecker.IsUserSafe(contexts.User.Id, contexts.Server.Id);
            return userIsSafe switch
            {
                true when messagesCount < howManyMessagesCount => SpamProbability.None,
                false when messagesCount <= howManyMessagesCount / 2 => SpamProbability.Low,
                false when messagesCount < howManyMessagesCount => SpamProbability.Medium,
                _ => SpamProbability.Sure
            };
        }
    }
}
