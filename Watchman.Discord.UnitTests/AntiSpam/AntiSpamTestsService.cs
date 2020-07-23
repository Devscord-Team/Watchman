using System;
using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;
using Moq;
using Watchman.DomainModel.Messages.Queries;
using Watchman.DomainModel.Settings.ConfigurationItems;
using Watchman.DomainModel.Settings.Services;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    internal class AntiSpamTestsService
    {
        public readonly ServerMessagesCacheService ExampleServerMessages = new ServerMessagesCacheService();
        public const int DEFAULT_TEST_USER_ID = 1;

        public AntiSpamTestsService()
        {
            var exampleSmallMessages = new List<SmallMessage>
            {
                new SmallMessage("abcde", 1, DateTime.Now.AddSeconds(-30), GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage("abcdefg", 2, DateTime.Now.AddSeconds(-20), GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage("fgdfgfa", 1, DateTime.Now.AddSeconds(-20), GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage("fgdfgfa", 1, DateTime.Now.AddSeconds(-1), GetMessagesQuery.GET_ALL_SERVERS),
            };
            this.ExampleServerMessages.OverwriteMessages(exampleSmallMessages);
        }

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

        public (DiscordRequest request, Contexts contexts) CreateRequestAndContexts(string content)
        {
            var request = new DiscordRequest { OriginalMessage = content };
            var contexts = this.GetDefaultContexts();
            return (request, contexts);
        }

        public (DiscordRequest request, Contexts contexts) CreateRequestAndContexts(SmallMessage smallMessage)
        {
            var request = new DiscordRequest
            {
                OriginalMessage = smallMessage.Content,
                SentAt = smallMessage.SentAt
            };
            var contexts = this.GetDefaultContexts(smallMessage.UserId, smallMessage.ServerId);
            return (request, contexts);
        }

        private Contexts GetDefaultContexts(ulong? userId = null, ulong? serverId = null)
        {
            var contexts = new Contexts();
            contexts.SetContext(new UserContext(userId ?? DEFAULT_TEST_USER_ID, null, new List<UserRole>(), null, null, false));
            contexts.SetContext(new DiscordServerContext(serverId ?? GetMessagesQuery.GET_ALL_SERVERS, null, null, null, null));
            return contexts;
        }
    }
}