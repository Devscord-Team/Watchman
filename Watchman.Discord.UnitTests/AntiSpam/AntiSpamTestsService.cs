using System.Collections.Generic;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Moq;
using Watchman.DomainModel.Configuration.ConfigurationItems;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    internal class AntiSpamTestsService
    {
        public const int DEFAULT_TEST_USER_ID = 1;

        public static Mock<IConfigurationService> GetConfigurationsMock()
        {
            var configurationService = new Mock<IConfigurationService>();
            configurationService
                .Setup(x => x.GetConfigurationItem<PercentOfSimilarityBetweenMessagesToSuspectSpam>(It.IsAny<ulong>()))
                .Returns(new PercentOfSimilarityBetweenMessagesToSuspectSpam(0));
            configurationService
                .Setup(x => x.GetConfigurationItem<MinUpperLettersCount>(It.IsAny<ulong>()))
                .Returns(new MinUpperLettersCount(0));
            configurationService
                .Setup(x => x.GetConfigurationItem<HowLongIsShortTimeInSeconds>(It.IsAny<ulong>()))
                .Returns(new HowLongIsShortTimeInSeconds(0));
            configurationService
                .Setup(x => x.GetConfigurationItem<HowManyMessagesInShortTimeToBeSpam>(It.IsAny<ulong>()))
                .Returns(new HowManyMessagesInShortTimeToBeSpam(0));
            return configurationService;
        }

        public Contexts GetDefaultContexts(ulong? userId = null, ulong? serverId = null)
        {
            var contexts = new Contexts();
            contexts.SetContext(new UserContext(userId ?? DEFAULT_TEST_USER_ID, null, new List<UserRole>(), null, null, (_) => false, null));
            contexts.SetContext(new DiscordServerContext(serverId ?? 0, null, null, null, null, null, null));
            return contexts;
        }
    }
}