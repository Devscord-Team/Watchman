using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using NUnit.Framework;
using Watchman.Discord.Areas.Protection.Strategies;

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
    }
}
