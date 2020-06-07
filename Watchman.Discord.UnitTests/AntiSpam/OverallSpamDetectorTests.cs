using System;
using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;
using NUnit.Framework;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.DomainModel.Messages.Queries;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    [TestFixture]
    internal class OverallSpamDetectorTests : AntiSpamTestsBase
    {
        [Test]
        [TestCase("spam", "spam", "spam", "spam", 1000, SpamProbability.Medium)]
        [TestCase("aaaaaaaaaabbbbb", "https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", 20, SpamProbability.Sure)]
        [TestCase("link: https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", 20, SpamProbability.Sure)]
        public void OverallSpamDetectorStrategy_ShouldDetectSpam(string messageContent1, string messageContent2, string messageContent3, string messageContent4, int userMessagesCount, SpamProbability exceptedSpamProbability)
        {
            // Arrange
            UserMessagesCounter
                .Setup(x => x.CountUserMessages(DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(userMessagesCount);

            var lastMessage = CreateMessage(messageContent4);
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent2, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent3, DEFAULT_TEST_USER_ID, DateTime.Now)
            });
            var overallSpamDetector = OverallSpamDetectorStrategy.GetStrategyWithDefaultDetectors(serverMessages, UserMessagesCounter.Object);

            // Act
            var overallSpamProbability = overallSpamDetector.GetOverallSpamProbability(lastMessage);

            // Assert
            Assert.That(overallSpamProbability, Is.EqualTo(exceptedSpamProbability));
        }

        [Test]
        [TestCase("not spam", "something else", "something more else", "totally different", 10)]
        public void OverallSpamDetectorStrategy_ShouldNotDetectSpam(string messageContent1, string messageContent2, string messageContent3, string messageContent4, int userMessagesCount)
        {
            // Arrange
            UserMessagesCounter
                .Setup(x => x.CountUserMessages(DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(userMessagesCount);

            var lastMessage = CreateMessage(messageContent4);
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent2, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent3, DEFAULT_TEST_USER_ID, DateTime.Now)
            });
            var overallSpamDetector = OverallSpamDetectorStrategy.GetStrategyWithDefaultDetectors(serverMessages, UserMessagesCounter.Object);

            // Act
            var overallSpamProbability = overallSpamDetector.GetOverallSpamProbability(lastMessage);

            // Assert
            Assert.That(overallSpamProbability, Is.EqualTo(SpamProbability.None));
        }
    }
}
