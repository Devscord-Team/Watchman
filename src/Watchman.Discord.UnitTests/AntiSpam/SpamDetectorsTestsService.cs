using System;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;
using Moq;
using Watchman.DomainModel.Messages.Queries;
using Watchman.DomainModel.Settings.ConfigurationItems;
using Watchman.DomainModel.Settings.Services;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    internal class SpamDetectorsTestsService<T> where T : ISpamDetector
    {
        public SpamProbability GetSpamProbability(bool isUserSafe, params string[] messagesContent)
        {
            // Arrange
            var spamTestsService = new AntiSpamTestsService();
            var userSafetyChecker = new Mock<IUserSafetyChecker>();
            userSafetyChecker
                .Setup(x => x.IsUserSafe(AntiSpamTestsService.DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(isUserSafe);
            var configurationService = new Mock<IConfigurationService>();
            configurationService
                .Setup(x => x.GetConfigurationItem<PercentOfSimilarityBetweenMessagesToSuspectSpam>(It.IsAny<ulong>()))
                .Returns(new PercentOfSimilarityBetweenMessagesToSuspectSpam(GetMessagesQuery.GET_ALL_SERVERS));

            var (request, contexts) = spamTestsService.CreateRequestAndContexts(messagesContent.Last());
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(messagesContent.SkipLast(1).Select(x => new SmallMessage(x, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now, GetMessagesQuery.GET_ALL_SERVERS)));

            var needsConfiguration = typeof(T).GetConstructors().Any(x => x.GetParameters().Any(x => x.ParameterType == typeof(IConfigurationService)));
            var spamDetector = needsConfiguration 
                ? (T)Activator.CreateInstance(typeof(T), userSafetyChecker.Object, configurationService.Object)
                : (T)Activator.CreateInstance(typeof(T), userSafetyChecker.Object);

            // Act
            return spamDetector!.GetSpamProbability(serverMessages, request, contexts);
        }
    }
}
