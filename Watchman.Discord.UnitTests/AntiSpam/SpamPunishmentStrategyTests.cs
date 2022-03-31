using System;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Moq;
using NUnit.Framework;
using Watchman.Discord.Areas.AntiSpam.Services;
using Watchman.Discord.Areas.AntiSpam.Strategies;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    [TestFixture]
    internal class SpamPunishmentStrategyTests
    {
        [Test]
        [TestCase(0, SpamProbability.Sure)]
        public void SpamPunishmentStrategy_ShouldGiveWarn(int userWarnsCount, SpamProbability spamProbability)
        {
            // Arrange
            var punishmentsCachingService = new Mock<IPunishmentsCachingService>();
            punishmentsCachingService
                .Setup(x => x.GetUserWarnsCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, It.IsAny<DateTime>()))
                .Returns(userWarnsCount);
            var punishmentStrategy = new SpamPunishmentStrategy(punishmentsCachingService.Object);

            // Act
            var punishment = punishmentStrategy.GetPunishment(AntiSpamTestsService.DEFAULT_TEST_USER_ID, spamProbability);

            // Assert
            Assert.That(punishment.PunishmentOption, Is.EqualTo(PunishmentOption.Warn));
        }

        [Test]
        [TestCase(5, 0, SpamProbability.Sure)]
        [TestCase(3, 2, SpamProbability.Medium)]
        public void SpamPunishmentStrategy_ShouldGiveMute(int userWarnsCount, int userMutesCount, SpamProbability spamProbability)
        {
            // Arrange
            var punishmentsCachingService = new Mock<IPunishmentsCachingService>();
            punishmentsCachingService
                .Setup(x => x.GetUserWarnsCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, It.IsAny<DateTime>()))
                .Returns(userWarnsCount);
            punishmentsCachingService
                .Setup(x => x.GetUserMutesCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, It.IsAny<DateTime>()))
                .Returns(userMutesCount);
            var punishmentStrategy = new SpamPunishmentStrategy(punishmentsCachingService.Object);

            // Act
            var punishment = punishmentStrategy.GetPunishment(AntiSpamTestsService.DEFAULT_TEST_USER_ID, spamProbability);

            // Assert
            Assert.That(punishment.PunishmentOption, Is.EqualTo(PunishmentOption.Mute));
        }

        [Test]
        [TestCase(0, SpamProbability.None)]
        [TestCase(0, SpamProbability.Low)]
        public void SpamPunishmentStrategy_ShouldNotGiveAnyPunishment(int userWarnsCount, SpamProbability spamProbability)
        {
            // Arrange
            var punishmentsCachingService = new Mock<IPunishmentsCachingService>();
            punishmentsCachingService
                .Setup(x => x.GetUserWarnsCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, It.IsAny<DateTime>()))
                .Returns(userWarnsCount);
            var punishmentStrategy = new SpamPunishmentStrategy(punishmentsCachingService.Object);

            // Act
            var punishment = punishmentStrategy.GetPunishment(AntiSpamTestsService.DEFAULT_TEST_USER_ID, spamProbability);

            // Assert
            Assert.That(punishment.PunishmentOption, Is.EqualTo(PunishmentOption.Nothing));
        }

        [Test]
        [TestCase(0, 15)]
        [TestCase(1, 30)]
        [TestCase(4, 240)]
        [TestCase(40, 35791)]
        public void SpamPunishmentStrategy_ShouldGiveMuteForTime(int userMutesCount, int expectedMinutesOfMute)
        {
            // Arrange
            var punishmentsCachingService = new Mock<IPunishmentsCachingService>();
            punishmentsCachingService
                .Setup(x => x.GetUserWarnsCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, It.IsAny<DateTime>()))
                .Returns(100); // to be sure that strategy always will give mute
            punishmentsCachingService
                .Setup(x => x.GetUserMutesCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, It.IsAny<DateTime>()))
                .Returns(userMutesCount);
            var punishmentStrategy = new SpamPunishmentStrategy(punishmentsCachingService.Object);

            // Act
            var punishment = punishmentStrategy.GetPunishment(AntiSpamTestsService.DEFAULT_TEST_USER_ID, SpamProbability.Sure);

            // Assert
            Assert.That(punishment.ForTime, Is.Not.Null);
            Assert.That(punishment.ForTime.Value.TotalMinutes, Is.EqualTo(expectedMinutesOfMute));
        }
    }
}
