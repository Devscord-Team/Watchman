using Devscord.DiscordFramework.Commands.AntiSpam;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Areas.AntiSpam.Services;
using Watchman.Discord.Areas.AntiSpam.Strategies;
using Watchman.Discord.Areas.Muting.Controllers;
using Watchman.Discord.Areas.Muting.Services;
using Watchman.Discord.UnitTests.TestObjectFactories;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.UnitTests.AntiSpam
{
    [TestFixture]
    internal class AntiSpamControllerTests
    {
        private readonly TestControllersFactory testControllersFactory = new();
        private readonly TestContextsFactory testContextsFactory = new();

        [Test]

        [TestCase(1u, SpamProbability.None, false, PunishmentOption.Nothing, false)]
        [TestCase(11u, SpamProbability.None, false, PunishmentOption.Warn, false)]
        [TestCase(111u, SpamProbability.None, false, PunishmentOption.Mute, false)]

        [TestCase(2u, SpamProbability.Low, true, PunishmentOption.Nothing, false)]
        [TestCase(22u, SpamProbability.Low, true, PunishmentOption.Warn, true)]
        [TestCase(222u, SpamProbability.Low, true, PunishmentOption.Mute, true)]

        [TestCase(3u, SpamProbability.Medium, true, PunishmentOption.Nothing, false)]
        [TestCase(33u, SpamProbability.Medium, true, PunishmentOption.Warn, true)]
        [TestCase(333u, SpamProbability.Medium, true, PunishmentOption.Mute, true)]

        [TestCase(4u, SpamProbability.Sure, true, PunishmentOption.Nothing, false)]
        [TestCase(44u, SpamProbability.Sure, true, PunishmentOption.Warn, true)]
        [TestCase(444u, SpamProbability.Sure, true, PunishmentOption.Mute, true)]
        public async Task Scan_ShouldPunishIfDetectorsFoundAnything(
            ulong userId, 
            SpamProbability detectedSpamProbability, 
            bool shouldInvokeSetPunishment,
            PunishmentOption punishmentOption,
            bool shouldAddPunishmentToUser)
        {
            //Arrange
            var contexts = this.testContextsFactory.CreateContexts(1, 1, userId: userId);
            var request = new DiscordRequest();

            var serverMessagesCacheServiceMock = new Mock<IServerMessagesCacheService>();

            var overallSpamDetectorMock = new Mock<IOverallSpamDetector>();
            overallSpamDetectorMock.Setup(x => x.GetOverallSpamProbability(It.IsAny<Contexts>()))
                .Returns(detectedSpamProbability);

            var overallSpamDetectorStrategyFactoryMock = new Mock<IOverallSpamDetectorStrategyFactory>();
            overallSpamDetectorStrategyFactoryMock
                .Setup(x => x.GetStrategyWithDefaultDetectors(
                    It.IsAny<IServerMessagesCacheService>(),
                    //It.IsAny<IUserSafetyChecker>(),
                    It.IsAny<IConfigurationService>()))
                .Returns(overallSpamDetectorMock.Object);

            var punishment = new Punishment(punishmentOption, DateTime.UtcNow, TimeSpan.FromSeconds(15));
            var spamPunishmentStrategyMock = new Mock<ISpamPunishmentStrategy>();
            spamPunishmentStrategyMock.Setup(x => x.GetPunishment(It.IsAny<ulong>(), It.IsAny<SpamProbability>()))
                .Returns(punishment);

            var antiSpamServiceMock = new Mock<IAntiSpamService>();

            var punishmentsCachingServiceMock = new Mock<IPunishmentsCachingService>();

            var controller = this.testControllersFactory.CreateAntiSpamController(
                serverMessagesCacheServiceMock: serverMessagesCacheServiceMock,
                overallSpamDetectorStrategyFactoryMock: overallSpamDetectorStrategyFactoryMock,
                antiSpamServiceMock: antiSpamServiceMock,
                spamPunishmentStrategyMock: spamPunishmentStrategyMock,
                punishmentsCachingServiceMock: punishmentsCachingServiceMock);

            //Act
            await controller.Scan(request, contexts);

            //Assert
            serverMessagesCacheServiceMock.Verify(x => x.AddMessage(request, contexts), Times.Once);
            overallSpamDetectorMock.Verify(x => x.GetOverallSpamProbability(contexts), Times.Once);
            spamPunishmentStrategyMock.Verify(x => x.GetPunishment(contexts.User.Id, detectedSpamProbability), shouldInvokeSetPunishment ? Times.Once : Times.Never);
            antiSpamServiceMock.Verify(x => x.SetPunishment(contexts, It.IsAny<Punishment>()), shouldInvokeSetPunishment ? Times.Once : Times.Never);
            punishmentsCachingServiceMock.Verify(x => x.AddUserPunishment(contexts.User.Id, punishment), shouldAddPunishmentToUser ? Times.Once : Times.Never);
        }
    }
}
