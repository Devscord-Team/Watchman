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
        public async Task Scan_ShouldNotPunishIfDetectorsNotFoundAnything()
        {
            //Arrange
            var contexts = this.testContextsFactory.CreateContexts(1, 1, 1);
            var request = new DiscordRequest();

            var serverMessagesCacheServiceMock = new Mock<IServerMessagesCacheService>();

            var overallSpamDetectorMock = new Mock<IOverallSpamDetector>();
            overallSpamDetectorMock.Setup(x => x.GetOverallSpamProbability(It.IsAny<Contexts>()))
                .Returns(SpamProbability.None);

            var overallSpamDetectorStrategyFactoryMock = new Mock<IOverallSpamDetectorStrategyFactory>();
            overallSpamDetectorStrategyFactoryMock
                .Setup(x => x.GetStrategyWithDefaultDetectors(
                    It.IsAny<IServerMessagesCacheService>(),
                    It.IsAny<IUserSafetyChecker>(),
                    It.IsAny<IConfigurationService>()))
                .Returns(overallSpamDetectorMock.Object);

            var antiSpamServiceMock = new Mock<IAntiSpamService>();

            var controller = this.testControllersFactory.CreateAntiSpamController(
                serverMessagesCacheServiceMock: serverMessagesCacheServiceMock,
                overallSpamDetectorStrategyFactoryMock: overallSpamDetectorStrategyFactoryMock,
                antiSpamServiceMock: antiSpamServiceMock);

            //Act
            await controller.Scan(request, contexts);

            //Assert
            serverMessagesCacheServiceMock.Verify(x => x.AddMessage(request, contexts), Times.Once);
            overallSpamDetectorMock.Verify(x => x.GetOverallSpamProbability(contexts), Times.Once);
            antiSpamServiceMock.Verify(x => x.SetPunishment(It.IsAny<Contexts>(), It.IsAny<Punishment>()), Times.Never);
        }
    }
}
