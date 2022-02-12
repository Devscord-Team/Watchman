using System;
using System.Collections.Generic;
using Devscord.DiscordFramework.Commands.AntiSpam;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;
using Moq;
using NUnit.Framework;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.DomainModel.Messages.Queries;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    [TestFixture]
    internal class OverallSpamDetectorTests
    {
        private readonly IConfigurationService _configurationService;
        private readonly AntiSpamTestsService _antiSpamTestsService;

        public OverallSpamDetectorTests()
        {
            this._antiSpamTestsService = new AntiSpamTestsService();
            this._configurationService = AntiSpamTestsService.GetConfigurationsMock().Object;
        }

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
            var userSafetyChecker = new Mock<IUserSafetyChecker>();
            userSafetyChecker
                .Setup(x => x.IsUserSafe(AntiSpamTestsService.DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(isUserSafe);
            
            var contexts = this._antiSpamTestsService.GetDefaultContexts();
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.UtcNow.AddSeconds(-20), GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage(messageContent2, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.UtcNow.AddSeconds(-16), GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage(messageContent3, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.UtcNow.AddSeconds(-15), GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage(messageContent4, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.UtcNow.AddSeconds(-10), GetMessagesQuery.GET_ALL_SERVERS)
            });
            var overallSpamDetector = OverallSpamDetectorStrategy.GetStrategyWithDefaultDetectors(serverMessages, userSafetyChecker.Object, this._configurationService);
            
            // Act
            var overallSpamProbability = overallSpamDetector.GetOverallSpamProbability(contexts);

            // Assert
            Assert.That(overallSpamProbability, Is.EqualTo(exceptedSpamProbability));
        }

        [Test]
        [TestCase("not spam", "something else", "something more else", "totally different", false)]
        [TestCase("not spam", "something else", "something more else", "totally different", true)]
        [TestCase("not spam", "something else", "something more else", "😃", true)]
        [TestCase("not spam", "something else", "something more else", "😃", false)]
        [TestCase("not spam", "something else", "something more else", ":smile:", true)]
        [TestCase("not spam", "something else", "something more else", ":smile:", false)]
        public void OverallSpamDetectorStrategy_ShouldNotDetectSpam(string messageContent1, string messageContent2, string messageContent3, string messageContent4, bool isUserSafe)
        {
            // Arrange
            var userSafetyChecker = new Mock<IUserSafetyChecker>();
            userSafetyChecker
                .Setup(x => x.IsUserSafe(AntiSpamTestsService.DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(isUserSafe);

            var contexts = this._antiSpamTestsService.GetDefaultContexts();
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.UtcNow.AddSeconds(-20), GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage(messageContent2, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.UtcNow.AddSeconds(-16), GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage(messageContent3, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.UtcNow.AddSeconds(-15), GetMessagesQuery.GET_ALL_SERVERS),
                new SmallMessage(messageContent4, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.UtcNow.AddSeconds(-10), GetMessagesQuery.GET_ALL_SERVERS)
            });
            var overallSpamDetector = OverallSpamDetectorStrategy.GetStrategyWithDefaultDetectors(serverMessages, userSafetyChecker.Object, this._configurationService);

            // Act
            var overallSpamProbability = overallSpamDetector.GetOverallSpamProbability(contexts);

            // Assert
            Assert.That(overallSpamProbability, Is.EqualTo(SpamProbability.None));
        }
    }
}
