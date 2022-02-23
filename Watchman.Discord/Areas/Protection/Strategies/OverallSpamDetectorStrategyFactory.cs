using System.Collections.Generic;
using Devscord.DiscordFramework.Commands.AntiSpam;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.Areas.Protection.Strategies
{
    public interface IOverallSpamDetectorStrategyFactory
    {
        IOverallSpamDetector GetStrategyWithDefaultDetectors(IServerMessagesCacheService serverMessagesCacheService, /*IUserSafetyChecker userSafetyChecker,*/ IConfigurationService configurationService);
    }

    public class OverallSpamDetectorStrategyFactory : IOverallSpamDetectorStrategyFactory
    {
        public IOverallSpamDetector GetStrategyWithDefaultDetectors(IServerMessagesCacheService serverMessagesCacheService, /*IUserSafetyChecker userSafetyChecker,*/ IConfigurationService configurationService)
        {
            return new OverallSpamDetectorStrategy(serverMessagesCacheService, new List<ISpamDetector>
            {
                new LinksDetectorStrategy(/*userSafetyChecker*/),
                new DuplicatedMessagesDetectorStrategy(/*userSafetyChecker*/ configurationService),
                new CapslockDetectorStrategy(/*userSafetyChecker*/ configurationService),
                new FloodDetectorStrategy(/*userSafetyChecker*/ configurationService)
            });
        }
    }
}
