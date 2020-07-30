using System.Collections.Generic;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Moq;
using Watchman.DomainModel.Messages.Queries;
using Watchman.DomainModel.Settings.ConfigurationItems;
using Watchman.DomainModel.Settings.Services;

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
                .Returns(new PercentOfSimilarityBetweenMessagesToSuspectSpam(GetMessagesQuery.GET_ALL_SERVERS));
            configurationService
                .Setup(x => x.GetConfigurationItem<MinUpperLettersCount>(It.IsAny<ulong>()))
                .Returns(new MinUpperLettersCount(GetMessagesQuery.GET_ALL_SERVERS));
            configurationService
                .Setup(x => x.GetConfigurationItem<HowLongIsShortTimeInSeconds>(It.IsAny<ulong>()))
                .Returns(new HowLongIsShortTimeInSeconds(GetMessagesQuery.GET_ALL_SERVERS));
            configurationService
                .Setup(x => x.GetConfigurationItem<HowManyMessagesInShortTimeToBeSpam>(It.IsAny<ulong>()))
                .Returns(new HowManyMessagesInShortTimeToBeSpam(GetMessagesQuery.GET_ALL_SERVERS));
            return configurationService;
        }

        public Contexts GetDefaultContexts(ulong? userId = null, ulong? serverId = null)
        {
            var contexts = new Contexts();
            contexts.SetContext(new UserContext(userId ?? DEFAULT_TEST_USER_ID, null, new List<UserRole>(), null, null, (_) => false, null));
            contexts.SetContext(new DiscordServerContext(serverId ?? GetMessagesQuery.GET_ALL_SERVERS, null, null, null, null, null, null));
            return contexts;
        }
    }
}