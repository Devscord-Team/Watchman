﻿using System;
using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;
using Moq;
using NUnit.Framework;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.DomainModel.Messages.Queries;
using Watchman.DomainModel.Settings.ConfigurationItems;
using Watchman.DomainModel.Settings.Services;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    [TestFixture]
    internal class OverallSpamDetectorTests
    {
        [Test]
        [TestCase("abc", "xyz", "spam", "spam", false, SpamProbability.Low)]
        [TestCase("xyz", "aaa", "abc", "https://discord.gg/example", true, SpamProbability.Low)]
        [TestCase("abc", "xyz", "spam", "spam", true, SpamProbability.Low)]
        [TestCase("xyz", "aaa", "abc", "https://discord.gg/example", false, SpamProbability.Medium)]
        [TestCase("spam", "spam", "spam", "spam", true, SpamProbability.Medium)]
        [TestCase("aaaaaaaaaabbbbb", "https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", false, SpamProbability.Sure)]
        [TestCase("link: https://discord.gg/example", "abc: https://discord.gg/example", "xyz: https://discord.gg/example", "spam: https://discord.gg/example", false, SpamProbability.Sure)]
        public void OverallSpamDetectorStrategy_ShouldDetectSpam(string messageContent1, string messageContent2, string messageContent3, string messageContent4, bool isUserSafe, SpamProbability exceptedSpamProbability)
        {
            // Arrange
            var spamTestsService = new AntiSpamTestsService();
            var userSafetyChecker = new Mock<IUserSafetyChecker>();
            userSafetyChecker
                .Setup(x => x.IsUserSafe(AntiSpamTestsService.DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(isUserSafe);
            
            var (request, contexts) = spamTestsService.CreateRequestAndContexts(messageContent4);
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now, GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage(messageContent2, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now, GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage(messageContent3, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now, GetMessagesQuery.GET_ALL_SERVERS)
            });
            var configurationService = new Mock<IConfigurationService>();
            configurationService
                .Setup(x => x.GetConfigurationItem<PercentOfSimilarityBetweenMessagesToSuspectSpam>(It.IsAny<ulong>()))
                .Returns(new PercentOfSimilarityBetweenMessagesToSuspectSpam(GetMessagesQuery.GET_ALL_SERVERS));
            var overallSpamDetector = OverallSpamDetectorStrategy.GetStrategyWithDefaultDetectors(serverMessages, userSafetyChecker.Object, configurationService.Object);

            // Act
            var overallSpamProbability = overallSpamDetector.GetOverallSpamProbability(request, contexts);

            // Assert
            Assert.That(overallSpamProbability, Is.EqualTo(exceptedSpamProbability));
        }

        [Test]
        [TestCase("not spam", "something else", "something more else", "totally different", false)]
        [TestCase("not spam", "something else", "something more else", "totally different", true)]
        public void OverallSpamDetectorStrategy_ShouldNotDetectSpam(string messageContent1, string messageContent2, string messageContent3, string messageContent4, bool isUserSafe)
        {
            // Arrange
            var spamTestsService = new AntiSpamTestsService();
            var userSafetyChecker = new Mock<IUserSafetyChecker>();
            userSafetyChecker
                .Setup(x => x.IsUserSafe(AntiSpamTestsService.DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(isUserSafe);

            var (request, contexts) = spamTestsService.CreateRequestAndContexts(messageContent4);
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now, GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage(messageContent2, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now, GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage(messageContent3, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now, GetMessagesQuery.GET_ALL_SERVERS)
            });
            var configurationService = new Mock<IConfigurationService>();
            configurationService
                .Setup(x => x.GetConfigurationItem<PercentOfSimilarityBetweenMessagesToSuspectSpam>(It.IsAny<ulong>()))
                .Returns(new PercentOfSimilarityBetweenMessagesToSuspectSpam(GetMessagesQuery.GET_ALL_SERVERS));
            var overallSpamDetector = OverallSpamDetectorStrategy.GetStrategyWithDefaultDetectors(serverMessages, userSafetyChecker.Object, configurationService.Object);

            // Act
            var overallSpamProbability = overallSpamDetector.GetOverallSpamProbability(request, contexts);

            // Assert
            Assert.That(overallSpamProbability, Is.EqualTo(SpamProbability.None));
        }
    }
}
