using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;
using Moq;
using NUnit.Framework;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.DomainModel.Messages.Queries;

namespace Watchman.Discord.UnitTests
{
    [TestFixture]
    public class AntiSpamDetectionTests
    {
        private const int DEFAULT_TEST_USER_ID = 1;
        private readonly ServerMessagesCacheService _exampleServerMessages = new ServerMessagesCacheService();
        private readonly Mock<IUserMessagesCounter> _userMessagesCounter = new Mock<IUserMessagesCounter>();

        public AntiSpamDetectionTests()
        {
            var exampleSmallMessages = new List<SmallMessage>
            {
                new SmallMessage("abcde", 1, DateTime.Now.AddSeconds(-30)),
                new SmallMessage("abcdefg", 2, DateTime.Now.AddSeconds(-20)),
                new SmallMessage("fgdfgfa", 1, DateTime.Now.AddSeconds(-20)),
                new SmallMessage("fgdfgfa", 1, DateTime.Now.AddSeconds(-1)),
            };
            _exampleServerMessages.OverwriteMessages(exampleSmallMessages);
        }

        [Test]
        [TestCase("some link: https://discord.com abc", 10, SpamProbability.Medium)]
        [TestCase("http://abc.com", 600, SpamProbability.Low)]
        public void LinksDetector_ShouldDetectSpam(string messageContent, int userMessagesCount, SpamProbability shouldDetectProb)
        {
            // Arrange
            var message = CreateMessage(messageContent);
            _userMessagesCounter
                .Setup(x => x.CountUserMessages(DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(userMessagesCount);
            var linksDetector = new LinksDetectorStrategy(_userMessagesCounter.Object);

            // Act
            var spamProbability = linksDetector.GetSpamProbability(_exampleServerMessages, message);

            // Assert
            Assert.That(spamProbability, Is.EqualTo(shouldDetectProb));
        }

        [Test]
        [TestCase("abcd")]
        [TestCase("rgdrg32http")]
        public void LinksDetector_ShouldNotDetectSpam(string messageContent)
        {
            // Arrange
            var message = CreateMessage(messageContent);
            var linksDetector = new LinksDetectorStrategy(_userMessagesCounter.Object);

            // Act
            var spamProbability = linksDetector.GetSpamProbability(_exampleServerMessages, message);

            // Assert
            Assert.That(spamProbability, Is.EqualTo(SpamProbability.None));
        }

        [Test]
        [TestCase("spam", "spam2", "spam3", "spam", 400)]
        [TestCase("spam", "spam2", "spam", "spam", 50)]
        [TestCase("aaaaaaaaaabbbbb", "https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", 20)]
        public void DuplicatesDetector_ShouldDetectSpam(string messageContent1, string messageContent2, string messageContent3, string messageContent4, int userMessagesCount)
        {
            // Arrange
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent2, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent3, DEFAULT_TEST_USER_ID, DateTime.Now)
            });
            _userMessagesCounter
                .Setup(x => x.CountUserMessages(DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(userMessagesCount);
            var duplicatesDetector = new DuplicatedMessagesDetectorStrategy()

            // Act


            // Assert

        }

        [Test]
        [TestCase("not spam", "something else", "something more else", "totally different")]
        [TestCase("hello", "zxy", "afegserg", "egr")]
        [TestCase("hello", "how", "are", "you")]
        public void DuplicatesDetector_ShouldNotDetectSpam(string messageContent1, string messageContent2, string messageContent3, string messageContent4)
        {
            // Arrange


            // Act


            // Assert

        }

        private Message CreateMessage(string content)
        {
            var request = new DiscordRequest { OriginalMessage = content };
            var contexts = GetDefaultContexts();
            var message = new Message(1, request, contexts);
            return message;
        }

        private Contexts GetDefaultContexts()
        {
            var contexts = new Contexts();
            contexts.SetContext(new UserContext(DEFAULT_TEST_USER_ID, "abc", new List<UserRole>(), null, null));
            contexts.SetContext(new DiscordServerContext(GetMessagesQuery.GET_ALL_SERVERS, "aaa", null, null, null));
            return contexts;
        }
    }
}