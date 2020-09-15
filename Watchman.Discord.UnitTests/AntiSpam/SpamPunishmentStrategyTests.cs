using System;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Moq;
using NUnit.Framework;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.Discord.Areas.Users.Services;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    [TestFixture]
    internal class SpamPunishmentStrategyTests
    {
        private readonly AntiSpamTestsService _antiSpamTestsService;

        public SpamPunishmentStrategyTests()
        {
            _antiSpamTestsService = new AntiSpamTestsService();
        }

        [Test]
        [TestCase(0, SpamProbability.Sure)]
        public void SpamPunishmentStrategy_ShouldGiveWarn(int userWarnsCount, SpamProbability spamProbability)
        {
            // Arrange
            var context = _antiSpamTestsService.GetDefaultContexts();
            var punishmentsCachingService = new Mock<IPunishmentsCachingService>();
            punishmentsCachingService
                .Setup(x => x.GetUserWarnsCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, It.IsAny<DateTime>()))
                .Returns(userWarnsCount);
            var warnsService = new Mock<IWarnsService>();
            warnsService
                .Setup(x => x.GetWarnsCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, context.Server.Id, It.IsAny<DateTime>()))
                .Returns(userWarnsCount);
            var punishmentStrategy = new SpamPunishmentStrategy(punishmentsCachingService.Object, warnsService.Object);

            // Act
            var punishment = punishmentStrategy.GetPunishment(context.User.Id, context.Server.Id, spamProbability);

            // Assert
            Assert.That(punishment.PunishmentOption, Is.EqualTo(PunishmentOption.Warn));
        }

        [Test]
        [TestCase(5, 0, SpamProbability.Sure)]
        [TestCase(3, 2, SpamProbability.Medium)]
        public void SpamPunishmentStrategy_ShouldGiveMute(int userWarnsCount, int userMutesCount, SpamProbability spamProbability)
        {
            // Arrange
            var context = _antiSpamTestsService.GetDefaultContexts();
            var punishmentsCachingService = new Mock<IPunishmentsCachingService>();
            punishmentsCachingService
                .Setup(x => x.GetUserWarnsCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, It.IsAny<DateTime>()))
                .Returns(userWarnsCount);
            punishmentsCachingService
                .Setup(x => x.GetUserMutesCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, It.IsAny<DateTime>()))
                .Returns(userMutesCount);
            var warnsService = new Mock<IWarnsService>();
            warnsService
                .Setup(x => x.GetWarnsCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, context.Server.Id, It.IsAny<DateTime>()))
                .Returns(userWarnsCount);
            var punishmentStrategy = new SpamPunishmentStrategy(punishmentsCachingService.Object, warnsService.Object);

            // Act
            var punishment = punishmentStrategy.GetPunishment(context.User.Id, context.Server.Id, spamProbability);

            // Assert
            Assert.That(punishment.PunishmentOption, Is.EqualTo(PunishmentOption.Mute));
        }

        [Test]
        [TestCase(0, SpamProbability.None)]
        [TestCase(0, SpamProbability.Low)]
        public void SpamPunishmentStrategy_ShouldNotGiveAnyPunishment(int userWarnsCount, SpamProbability spamProbability)
        {
            // Arrange
            var context = _antiSpamTestsService.GetDefaultContexts();
            var punishmentsCachingService = new Mock<IPunishmentsCachingService>();
            punishmentsCachingService
                .Setup(x => x.GetUserWarnsCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, It.IsAny<DateTime>()))
                .Returns(userWarnsCount);
            var warnsService = new Mock<IWarnsService>();
            warnsService
                .Setup(x => x.GetWarnsCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, context.Server.Id, It.IsAny<DateTime>()))
                .Returns(userWarnsCount);
            var punishmentStrategy = new SpamPunishmentStrategy(punishmentsCachingService.Object, warnsService.Object);

            // Act
            var punishment = punishmentStrategy.GetPunishment(context.User.Id, context.Server.Id, spamProbability);

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
            const int warnsCount = 100;
            var context = _antiSpamTestsService.GetDefaultContexts();
            var punishmentsCachingService = new Mock<IPunishmentsCachingService>();
            punishmentsCachingService
                .Setup(x => x.GetUserWarnsCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, It.IsAny<DateTime>()))
                .Returns(warnsCount); // to be sure that strategy always will give mute
            punishmentsCachingService
                .Setup(x => x.GetUserMutesCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, It.IsAny<DateTime>()))
                .Returns(userMutesCount);
            var warnsService = new Mock<IWarnsService>();
            warnsService
                .Setup(x => x.GetWarnsCount(AntiSpamTestsService.DEFAULT_TEST_USER_ID, context.Server.Id, It.IsAny<DateTime>()))
                .Returns(warnsCount);
            var punishmentStrategy = new SpamPunishmentStrategy(punishmentsCachingService.Object, warnsService.Object);

            // Act
            var punishment = punishmentStrategy.GetPunishment(context.User.Id, context.Server.Id, SpamProbability.Sure);

            // Assert
            Assert.That(punishment.ForTime, Is.Not.Null);
            Assert.That(punishment.ForTime.Value.TotalMinutes, Is.EqualTo(expectedMinutesOfMute));
        }
    }
}
