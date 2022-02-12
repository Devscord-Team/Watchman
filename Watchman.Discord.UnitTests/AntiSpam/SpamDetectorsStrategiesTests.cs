using System;
using System.Linq;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services.Models;
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
            var spamDetectorsTestsService = new SpamDetectorsTestsService<LinksDetectorStrategy>();
            var spamProbability = spamDetectorsTestsService.GetSpamProbability(isUserSafe, messageContent);

            // Assert
            Assert.That(spamProbability, Is.EqualTo(exceptedSpamProbability));
        }

        [Test]
        [TestCase("abcd", false)]
        [TestCase("abcd", true)]
        [TestCase("rgdrg32http", false)]
        [TestCase("rgdrg32http", true)]
        public void LinksDetector_ShouldNotDetectSpam(string messageContent, bool isUserSafe)
        {
            var spamDetectorsTestsService = new SpamDetectorsTestsService<LinksDetectorStrategy>();
            var spamProbability = spamDetectorsTestsService.GetSpamProbability(isUserSafe, messageContent);

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
            var spamDetectorsTestsService = new SpamDetectorsTestsService<DuplicatedMessagesDetectorStrategy>();
            var spamProbability = spamDetectorsTestsService.GetSpamProbability(isUserSafe, messageContent1, messageContent2, messageContent3, messageContent4);

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
            var spamDetectorsTestsService = new SpamDetectorsTestsService<DuplicatedMessagesDetectorStrategy>();
            var spamProbability = spamDetectorsTestsService.GetSpamProbability(isUserSafe, messageContent1, messageContent2, messageContent3, messageContent4);

            // Assert
            Assert.That(spamProbability, Is.EqualTo(SpamProbability.None));
        }

        [Test]
        [TestCase(SpamProbability.Low, "not capslock", "not capslock", "not capslock", "not capslock", "not capslock", "CAPSLOCK")]
        [TestCase(SpamProbability.Medium, "ALSO CAPSLOCK abc", "not capslock", "not capslock", "not capslock", "not capslock", "CAPSLOCK")]
        [TestCase(SpamProbability.Sure, "ALSO CAPSLOCK abc", "ANOTHER CAPSLOCK", "NOT capslock", "not capslock XDD", "not capslock", "CAPSLOCK")]
        public void CapslockDetector_ShouldDetectSpam(SpamProbability exceptedSpamProbability, string m1, string m2, string m3, string m4, string m5, string m6) // not using params to force 6 messages
        {
            var spamDetectorsTestsService = new SpamDetectorsTestsService<CapslockDetectorStrategy>();
            var spamProbability = spamDetectorsTestsService.GetSpamProbability(isUserSafe: false, m1, m2, m3, m4, m5, m6);

            // Assert
            Assert.That(spamProbability, Is.EqualTo(exceptedSpamProbability));
        }

        [Test]
        [TestCase("not capslock", "not capslock", "not capslock", "not capslock", "not capslock", "not capslock")]
        [TestCase("REST API", "not capslock", "not capslock", "not capslock", "NOT CApslock", "XDDD")]
        [TestCase("not capslock", "not capslock", "not capslock", "not capslock", "REST API", "XDDD")]
        [TestCase("not capslock", "not capslock", "not capslock", "not capslock", "REST API", "NOT CApslock")]
        public void CapslockDetector_ShouldNotDetectSpam(string m1, string m2, string m3, string m4, string m5, string m6) // not using params to force 6 messages
        {
            var spamDetectorsTestsService = new SpamDetectorsTestsService<CapslockDetectorStrategy>();
            var spamProbability = spamDetectorsTestsService.GetSpamProbability(isUserSafe: false, m1, m2, m3, m4, m5, m6);

            // Assert
            Assert.That(spamProbability, Is.EqualTo(SpamProbability.None));
        }

        [Test]
        [TestCase(SpamProbability.Low, false, 14, 1, 0)]
        [TestCase(SpamProbability.Low, false, 8, 6, 1)]
        [TestCase(SpamProbability.Medium, false, 10, 8, 6, 1)]
        [TestCase(SpamProbability.Medium, false, 25, 24, 23, 22, 21, 20, 19, 16, 10, 8, 6, 1)]
        [TestCase(SpamProbability.Sure, false, 15, 14, 13, 13, 13, 10, 8, 6, 1)]
        [TestCase(SpamProbability.Sure, true, 15, 14, 13, 13, 13, 10, 8, 6, 1)]
        public void FloodDetector_ShouldDetectSpam(SpamProbability expectedSpamProbability, bool isUserSafe, params int[] secondsBefore)
        {
            var spamDetectorsTestsService = new SpamDetectorsTestsService<FloodDetectorStrategy>();
            var smallMessages = secondsBefore.Select(x => new SmallMessage("test", AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.UtcNow.AddSeconds(-x), GetMessagesQuery.GET_ALL_SERVERS));
            var spamProbability = spamDetectorsTestsService.GetSpamProbability(isUserSafe, smallMessages.ToArray());

            // Assert
            Assert.That(spamProbability, Is.EqualTo(expectedSpamProbability));
        }

        [Test]
        [TestCase(false, 1)]
        [TestCase(true, 1)]
        [TestCase(false, 10, 1)]
        [TestCase(true, 10, 1)]
        [TestCase(true, 24, 23, 22, 21, 20, 19, 16, 10, 8, 6, 1)]
        [TestCase(true, 25, 24, 23, 22, 21, 16, 12, 11, 10, 8, 6, 1)]
        public void FloodDetector_ShouldNotDetectSpam(bool isUserSafe, params int[] secondsBefore)
        {
            var spamDetectorsTestsService = new SpamDetectorsTestsService<FloodDetectorStrategy>();
            var smallMessages = secondsBefore.Select(x => new SmallMessage("test", AntiSpamTestsService.DEFAULT_TEST_USER_ID, DateTime.UtcNow.AddSeconds(-x), GetMessagesQuery.GET_ALL_SERVERS));
            var spamProbability = spamDetectorsTestsService.GetSpamProbability(isUserSafe, smallMessages.ToArray());

            // Assert
            Assert.That(spamProbability, Is.EqualTo(SpamProbability.None));
        }
    }
}
