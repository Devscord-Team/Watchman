using System;
using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;
using Moq;
using NUnit.Framework;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.DomainModel.Messages.Queries;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    [TestFixture]
    internal class SpamDetectorsStrategiesTests
    {
        [Test]
        [TestCase("some link: https://discord.com abc", false, SpamProbability.Medium)]
        [TestCase("http://abc.com", true, SpamProbability.Low)]
        public void LinksDetector_ShouldDetectSpam(string messageContent, bool isUserSafe, SpamProbability exceptedSpamProbability)
        {
            // Arrange
            var spamTestsService = new AntiSpamTestsService();
            var (request, contexts) = spamTestsService.CreateRequestAndContexts(messageContent);
            var userSafetyChecker = new Mock<IUserSafetyChecker>();
            userSafetyChecker
                .Setup(x => x.IsUserSafe(AntiSpamTestsService.DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(isUserSafe);

            var linksDetector = new LinksDetectorStrategy(userSafetyChecker.Object);

            // Act
            var spamProbability = linksDetector.GetSpamProbability(spamTestsService.ExampleServerMessages, request, contexts);

            // Assert
            Assert.That(spamProbability, Is.EqualTo(exceptedSpamProbability));
        }

        [Test]
        [TestCase("abcd")]
        [TestCase("rgdrg32http")]
        public void LinksDetector_ShouldNotDetectSpam(string messageContent)
        {
            // Arrange
            var spamTestsService = new AntiSpamTestsService();
            var (request, contexts) = spamTestsService.CreateRequestAndContexts(messageContent);
            var userSafetyChecker = new Mock<IUserSafetyChecker>();
            var linksDetector = new LinksDetectorStrategy(userSafetyChecker.Object);

            // Act
            var spamProbability = linksDetector.GetSpamProbability(spamTestsService.ExampleServerMessages, request, contexts);

            // Assert
            Assert.That(spamProbability, Is.EqualTo(SpamProbability.None));
        }

        [Test]
        [TestCase("spam", "spam", "spam", "spam", true, SpamProbability.Medium)]
        [TestCase("spam", "sth else", "spam", "spam", true, SpamProbability.Low)]
        [TestCase("spam", "spam2", "spam3", "spamm", false, SpamProbability.Sure)]
        [TestCase("spam", "spam2", "spam3", "spamm", true, SpamProbability.Medium)]
        [TestCase("spam", "spam2", "spam", "spam", false, SpamProbability.Sure)]
        [TestCase("hello", "how", "her", "here", false, SpamProbability.Low)]
        [TestCase("hello", "how", "her", "here", true, SpamProbability.Low)]
        [TestCase("aaaaaaaaaabbbbb", "https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", false, SpamProbability.Medium)]
        [TestCase("link: https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", false, SpamProbability.Sure)]
        public void DuplicatesDetector_ShouldDetectSpam(string messageContent1, string messageContent2, string messageContent3, string messageContent4, bool isUserSafe, SpamProbability exceptedSpamProbability)
        {
            // Arrange
            var spamTestsService = new AntiSpamTestsService();
            var userSafetyChecker = new Mock<IUserSafetyChecker>();
            userSafetyChecker
                .Setup(x => x.IsUserSafe(AntiSpamTestsService.DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(isUserSafe);

            var duplicatesDetector = new DuplicatedMessagesDetectorStrategy(userSafetyChecker.Object);

            var (request, contexts) = spamTestsService.CreateRequestAndContexts(messageContent4);
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent2, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent3, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now)
            });

            // Act
            var spamProbability = duplicatesDetector.GetSpamProbability(serverMessages, request, contexts);

            // Assert
            Assert.That(spamProbability, Is.EqualTo(exceptedSpamProbability));
        }

        [Test]
        [TestCase("not spam", "something else", "something more else", "totally different", false)]
        [TestCase("hello", "zxy", "afegserg", "egr", false)]
        [TestCase("hello", "how", "are", "you", false)]
        [TestCase("hello", "how", "him", "you", true)]
        public void DuplicatesDetector_ShouldNotDetectSpam(string messageContent1, string messageContent2, string messageContent3, string messageContent4, bool isUserSafe)
        {
            // Arrange
            var spamTestsService = new AntiSpamTestsService();
            var userSafetyChecker = new Mock<IUserSafetyChecker>();
            userSafetyChecker
                .Setup(x => x.IsUserSafe(AntiSpamTestsService.DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(isUserSafe);

            var duplicatesDetector = new DuplicatedMessagesDetectorStrategy(userSafetyChecker.Object);

            var (request, contexts) = spamTestsService.CreateRequestAndContexts(messageContent4);
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent2, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent3, AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.Now)
            });

            // Act
            var spamProbability = duplicatesDetector.GetSpamProbability(serverMessages, request, contexts);

            // Assert
            Assert.That(spamProbability, Is.EqualTo(SpamProbability.None));
        }
    }
}
