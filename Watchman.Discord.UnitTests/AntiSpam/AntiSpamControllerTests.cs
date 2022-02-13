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
using Watchman.Discord.Areas.Protection.Controllers;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Protection.Strategies;
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
        [TestCase(1, SpamProbability.None, false)]
        [TestCase(2, SpamProbability.Low, true)]
        [TestCase(3, SpamProbability.Medium, true)]
        [TestCase(4, SpamProbability.Sure, true)]
        //userId is int because TestCase can't handle ulong in params
        public async Task Scan_ShouldPunishIfDetectorsFoundAnything(int userId, SpamProbability detectedSpamProbability, bool shouldInvokeSetPunishment)
        {
            //Arrange
            var contexts = this.testContextsFactory.CreateContexts(1, 1, userId: ulong.Parse(userId.ToString()));
            var request = new DiscordRequest();

            var serverMessagesCacheServiceMock = new Mock<IServerMessagesCacheService>();

            var overallSpamDetectorMock = new Mock<IOverallSpamDetector>();
            overallSpamDetectorMock.Setup(x => x.GetOverallSpamProbability(It.IsAny<Contexts>()))
                .Returns(detectedSpamProbability);

            var overallSpamDetectorStrategyFactoryMock = new Mock<IOverallSpamDetectorStrategyFactory>();
            overallSpamDetectorStrategyFactoryMock
                .Setup(x => x.GetStrategyWithDefaultDetectors(
                    It.IsAny<IServerMessagesCacheService>(),
                    It.IsAny<IUserSafetyChecker>(),
                    It.IsAny<IConfigurationService>()))
                .Returns(overallSpamDetectorMock.Object);

            var spamPunishmentStrategyMock = new Mock<ISpamPunishmentStrategy>();
            spamPunishmentStrategyMock.Setup(x => x.GetPunishment(It.IsAny<ulong>(), It.IsAny<SpamProbability>()))
                .Returns(new Punishment(PunishmentOption.Warn, DateTime.UtcNow, TimeSpan.FromSeconds(15)));

            var antiSpamServiceMock = new Mock<IAntiSpamService>();

            var controller = this.testControllersFactory.CreateAntiSpamController(
                serverMessagesCacheServiceMock: serverMessagesCacheServiceMock,
                overallSpamDetectorStrategyFactoryMock: overallSpamDetectorStrategyFactoryMock,
                antiSpamServiceMock: antiSpamServiceMock,
                spamPunishmentStrategyMock: spamPunishmentStrategyMock);

            //Act
            await controller.Scan(request, contexts);

            //Assert
            serverMessagesCacheServiceMock.Verify(x => x.AddMessage(request, contexts), Times.Once);
            overallSpamDetectorMock.Verify(x => x.GetOverallSpamProbability(contexts), Times.Once);
            spamPunishmentStrategyMock.Verify(x => x.GetPunishment(contexts.User.Id, detectedSpamProbability), shouldInvokeSetPunishment ? Times.Once : Times.Never);
            antiSpamServiceMock.Verify(x => x.SetPunishment(contexts, It.IsAny<Punishment>()), shouldInvokeSetPunishment ? Times.Once : Times.Never);
        }
    }
}
