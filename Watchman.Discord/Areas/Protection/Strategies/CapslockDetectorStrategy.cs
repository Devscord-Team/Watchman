using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.DomainModel.Configuration.ConfigurationItems;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public class CapslockDetectorStrategy : ISpamDetector
    {
        private readonly IConfigurationService _configurationService;

        public CapslockDetectorStrategy(IUserSafetyChecker userSafetyChecker, IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
        }

        public SpamProbability GetSpamProbability(ServerMessagesCacheService serverMessagesCacheService, Contexts contexts)
        {
            var lastFewMessages = serverMessagesCacheService.GetLastUserMessages(contexts.User.Id, contexts.Server.Id)
                .TakeLast(7)
                .ToList();

            if (!this.IsMessageWithMuchCapslock(lastFewMessages.Last().Content, contexts.Server.Id))
            {
                return SpamProbability.None;
            }
            var capslockedMessagesCount = lastFewMessages.Count(x => this.IsMessageWithMuchCapslock(x.Content, contexts.Server.Id));
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
