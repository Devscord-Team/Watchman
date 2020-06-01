using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;
using NUnit.Framework;
using Watchman.Discord.Areas.Protection.Strategies;

namespace Watchman.Discord.UnitTests
{
    [TestFixture]
    public class AntiSpamDetectionTests
    {
        private readonly List<SmallMessage> _exampleServerMessages = new List<SmallMessage>
        {
            new SmallMessage("abcde", 1, DateTime.Now.AddSeconds(-30)),
            new SmallMessage("abcdefg", 2, DateTime.Now.AddSeconds(-20)),
            new SmallMessage("fgdfgfa", 1, DateTime.Now.AddSeconds(-20)),
            new SmallMessage("fgdfgfa", 1, DateTime.Now.AddSeconds(-1)),
        };
        private readonly List<ISpamDetector> _spamDetectors = new List<ISpamDetector>();

        public AntiSpamDetectionTests()
        {
            _spamDetectors.Add(new LinksDetectorStrategy());
        }

        [Test]
        [TestCase("some link: https://discord.com abc")]
        [TestCase("http://abc.com")]
        public void ShouldDetectSpam(string messageContent)
        {
            // Arrange
            var message = CreateMessage(messageContent);

            // Act
            var spamProbabilities = _spamDetectors.Select(x => x.GetSpamProbability(_exampleServerMessages, message));

            // Assert
            foreach (var spamProb in spamProbabilities)
            {
                Assert.That(spamProb, Is.AtLeast(SpamProbability.Low));
            }
        }

        [Test]
        [TestCase("abcd")]
        [TestCase("rgdrg32http")]
        public void ShouldNotDetectSpam(string messageContent)
        {
            // Arrange
            var message = CreateMessage(messageContent);

            // Act
            var spamProbabilities = _spamDetectors.Select(x => x.GetSpamProbability(_exampleServerMessages, message));

            // Assert
            foreach (var spamProb in spamProbabilities)
            {
                Assert.That(spamProb, Is.EqualTo(SpamProbability.None));
            }
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
            contexts.SetContext(new UserContext(1, "abc", new List<UserRole>(), null, null));
            return contexts;
        }
    }
}