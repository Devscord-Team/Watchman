using AutoFixture.NUnit3;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Muting.Services;
using Watchman.Discord.Areas.Muting.Services.Commands;
using Watchman.Discord.UnitTests.TestObjectFactories;
using Watchman.DomainModel.Muting;

namespace Watchman.Discord.UnitTests.Muting
{
    [TestFixture]
    internal class AntiSpamServiceTests
    {
        private readonly TestContextsFactory testContextsFactory = new();

        [Test, AutoData]
        public async Task SetPunishment_ShouldDoNothingWhenPunishmentOptionIsNothing(DateTime givenAt)
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);
            var punishment = new Punishment(PunishmentOption.Nothing, givenAt);

            var commandBusMock = new Mock<ICommandBus>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            var unmutingServiceMock = new Mock<IUnmutingService>();

            var service = new AntiSpamService(commandBusMock.Object, messagesServiceFactoryMock.Object, unmutingServiceMock.Object);

            //Act
            await service.SetPunishment(contexts, punishment);

            //Assert
            messagesServiceFactoryMock.Verify(x => x.Create(contexts), Times.Never);
            commandBusMock.Verify(x => x.ExecuteAsync(It.IsAny<MuteUserOrOverwriteCommand>()), Times.Never);
            unmutingServiceMock.Verify(x => x.UnmuteInFuture(contexts, It.IsAny<MuteEvent>(), It.IsAny<UserContext>()), Times.Never);
        }

        [Test, AutoData]
        public async Task SetPunishment_ShouldSendResponseThatSpamAlertRecognizedWhenPunishmentOptionIsWarn(DateTime givenAt)
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);
            var punishment = new Punishment(PunishmentOption.Warn, givenAt);

            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);

            var service = new AntiSpamService(commandBus: null, messagesServiceFactoryMock.Object, unmutingService: null);

            //Act
            await service.SetPunishment(contexts, punishment);

            //Assert
            messagesServiceMock.Verify(x => x.SendResponse(It.IsAny<Func<IResponsesService, string>>()), Times.Once);
        }

        [Test, AutoData]
        public async Task SetPunishment_ShouldMuteUserForSpamWhenPunishmentOptionIsMute(DateTime givenAt, TimeSpan forTime)
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);
            var punishment = new Punishment(PunishmentOption.Mute, givenAt, forTime);

            var commandBusMock = new Mock<ICommandBus>();
            var unmutingServiceMock = new Mock<IUnmutingService>();

            var service = new AntiSpamService(commandBusMock.Object, messagesServiceFactory: null, unmutingServiceMock.Object);

            //Act
            await service.SetPunishment(contexts, punishment);

            //Assert
            commandBusMock.Verify(x => x.ExecuteAsync(It.IsAny<MuteUserOrOverwriteCommand>()), Times.Once);
            unmutingServiceMock.Verify(x => x.UnmuteInFuture(contexts, It.IsAny<MuteEvent>(), It.IsAny<UserContext>()), Times.Once);
        }
    }
}
