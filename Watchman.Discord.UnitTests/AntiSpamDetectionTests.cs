using System;
using System.Collections.Generic;
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
        private const int DEFAULT_COUNT_TO_BE_SAFE = 500;
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
        public void LinksDetector_ShouldDetectSpam(string messageContent, int userMessagesCount, SpamProbability exceptedSpamProbability)
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
            Assert.That(spamProbability, Is.EqualTo(exceptedSpamProbability));
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
        [TestCase("spam", "spam", "spam", "spam", 1000, SpamProbability.Medium)]
        [TestCase("spam", "sth else", "spam", "spam", 1000, SpamProbability.Low)]
        [TestCase("spam", "spam2", "spam3", "spamm", 400, SpamProbability.Sure)]
        [TestCase("spam", "spam2", "spam3", "spamm", 1200, SpamProbability.Medium)]
        [TestCase("spam", "spam2", "spam", "spam", 50, SpamProbability.Sure)]
        [TestCase("aaaaaaaaaabbbbb", "https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", 20, SpamProbability.Medium)]
        [TestCase("link: https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", 20, SpamProbability.Sure)]
        public void DuplicatesDetector_ShouldDetectSpam(string messageContent1, string messageContent2, string messageContent3, string messageContent4, int userMessagesCount, SpamProbability exceptedSpamProbability)
        {
            // Arrange
            _userMessagesCounter
                .Setup(x => x.CountUserMessages(DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(userMessagesCount);
            _userMessagesCounter
                .Setup(x => x.UserMessagesCountToBeSafe)
                .Returns(DEFAULT_COUNT_TO_BE_SAFE);
            
            var duplicatesDetector = new DuplicatedMessagesDetectorStrategy(_userMessagesCounter.Object);

            var lastMessage = CreateMessage(messageContent4);
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent2, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent3, DEFAULT_TEST_USER_ID, DateTime.Now)
            });

            // Act
            var spamProbability = duplicatesDetector.GetSpamProbability(serverMessages, lastMessage);

            // Assert
            Assert.That(spamProbability, Is.EqualTo(exceptedSpamProbability));
        }

        [Test]
        [TestCase("not spam", "something else", "something more else", "totally different", 10)]
        [TestCase("hello", "zxy", "afegserg", "egr", 5)]
        [TestCase("hello", "how", "are", "you", 5)]
        [TestCase("hello", "how", "her", "shower", 50)]
        public void DuplicatesDetector_ShouldNotDetectSpam(string messageContent1, string messageContent2, string messageContent3, string messageContent4, int userMessagesCount)
        {
            // Arrange
            _userMessagesCounter
                .Setup(x => x.CountUserMessages(DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(userMessagesCount);
            _userMessagesCounter
                .Setup(x => x.UserMessagesCountToBeSafe)
                .Returns(DEFAULT_COUNT_TO_BE_SAFE);
            
            var duplicatesDetector = new DuplicatedMessagesDetectorStrategy(_userMessagesCounter.Object);

            var lastMessage = CreateMessage(messageContent4);
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent2, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent3, DEFAULT_TEST_USER_ID, DateTime.Now)
            });

            // Act
            var spamProbability = duplicatesDetector.GetSpamProbability(serverMessages, lastMessage);

            // Assert
            Assert.That(spamProbability, Is.EqualTo(SpamProbability.None));
        }

        [Test]
        [TestCase("spam", "spam", "spam", "spam", 1000, SpamProbability.Medium)]
        [TestCase("aaaaaaaaaabbbbb", "https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", 20, SpamProbability.Sure)]
        [TestCase("link: https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", "https://discord.gg/example", 20, SpamProbability.Sure)]
        public void OverallSpamDetectorStrategy_ShouldDetectSpam(string messageContent1, string messageContent2, string messageContent3, string messageContent4, int userMessagesCount, SpamProbability exceptedSpamProbability)
        {
            // Arrange
                _userMessagesCounter
                .Setup(x => x.CountUserMessages(DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
                .Returns(userMessagesCount);
            _userMessagesCounter
                .Setup(x => x.UserMessagesCountToBeSafe)
                .Returns(DEFAULT_COUNT_TO_BE_SAFE);

            var lastMessage = CreateMessage(messageContent4);
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent2, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent3, DEFAULT_TEST_USER_ID, DateTime.Now)
            });
            var spamDetectors = new List<ISpamDetector>
            {
                new LinksDetectorStrategy(_userMessagesCounter.Object),
                new DuplicatedMessagesDetectorStrategy(_userMessagesCounter.Object)
            };
            var overallSpamDetector = new OverallSpamDetectorStrategy(serverMessages, spamDetectors);

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
            _userMessagesCounter
            .Setup(x => x.CountUserMessages(DEFAULT_TEST_USER_ID, GetMessagesQuery.GET_ALL_SERVERS))
            .Returns(userMessagesCount);
            _userMessagesCounter
                .Setup(x => x.UserMessagesCountToBeSafe)
                .Returns(DEFAULT_COUNT_TO_BE_SAFE);

            var lastMessage = CreateMessage(messageContent4);
            var serverMessages = new ServerMessagesCacheService();
            serverMessages.OverwriteMessages(new List<SmallMessage>
            {
                new SmallMessage(messageContent1, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent2, DEFAULT_TEST_USER_ID, DateTime.Now),
                new SmallMessage(messageContent3, DEFAULT_TEST_USER_ID, DateTime.Now)
            });
            var spamDetectors = new List<ISpamDetector>
            {
                new LinksDetectorStrategy(_userMessagesCounter.Object),
                new DuplicatedMessagesDetectorStrategy(_userMessagesCounter.Object)
            };
            var overallSpamDetector = new OverallSpamDetectorStrategy(serverMessages, spamDetectors);

            // Act
            var overallSpamProbability = overallSpamDetector.GetOverallSpamProbability(lastMessage);

            // Assert
            Assert.That(overallSpamProbability, Is.EqualTo(SpamProbability.None));
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
            contexts.SetContext(new UserContext(DEFAULT_TEST_USER_ID, null, new List<UserRole>(), null, null));
            contexts.SetContext(new DiscordServerContext(GetMessagesQuery.GET_ALL_SERVERS, null, null, null, null));
            return contexts;
        }
    }
}