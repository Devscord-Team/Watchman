using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.DomainModel.Settings.ConfigurationItems;
using Watchman.DomainModel.Settings.Services;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class CapslockDetectorStrategy : ISpamDetector
    {
        private readonly IConfigurationService _configurationService;
        public IUserSafetyChecker UserSafetyChecker { get; set; }

        public CapslockDetectorStrategy(IUserSafetyChecker userSafetyChecker, IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
            this.UserSafetyChecker = userSafetyChecker;
        }

        public SpamProbability GetSpamProbability(ServerMessagesCacheService serverMessagesCacheService, DiscordRequest request, Contexts contexts)
        {
            if (!this.IsMessageWithMuchCapslock(request.OriginalMessage, contexts.Server.Id))
            {
                return SpamProbability.None;
            }
            var lastFewMessages = serverMessagesCacheService.GetLastUserMessages(contexts.User.Id, contexts.Server.Id)
                .TakeLast(6)
                .ToList();

            var capslockedMessagesCount = lastFewMessages.Count(x => this.IsMessageWithMuchCapslock(x.Content, contexts.Server.Id));
            capslockedMessagesCount++; // now handled messages is also with capslock
            return capslockedMessagesCount switch
            {
                1 => SpamProbability.Low,
                2 => SpamProbability.Medium,
                _ => SpamProbability.Sure
            };
        }

        private bool IsMessageWithMuchCapslock(string message, ulong serverId)
        {
            message = message.Replace(" ", "");
            var upperLettersCount = message.Count(char.IsUpper);
            var percentageOfUpperLetters = upperLettersCount / (double)message.Length;
            var minUpperLettersCount = this._configurationService.GetConfigurationItem<MinUpperLettersCount>(serverId).Value;
            return upperLettersCount >= minUpperLettersCount && percentageOfUpperLetters >= 0.8;
        }
    }
}
