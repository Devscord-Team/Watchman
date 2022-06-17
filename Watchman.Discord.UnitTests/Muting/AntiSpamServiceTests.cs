using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.UnitTests.TestObjectFactories;
using AutoFixture.NUnit3;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Muting.Services;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Muting.Services.Commands;
using Watchman.DomainModel.Muting;
using Devscord.DiscordFramework.Commands.Responses;

namespace Watchman.Discord.UnitTests.Muting
{
    [TestFixture]
    internal class AntiSpamServiceTests
    {
        private readonly TestContextsFactory testContextsFactory = new();

        [Test, AutoData]
        public async Task SetPunishment_ShouldSendResponseThatSpamAlertRecognizedWhenPunishmentOptionIsWarn(DateTime dateTime)
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);
            var punishment = new Punishment(PunishmentOption.Warn, dateTime);

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
        public async Task SetPunishment_ShouldMuteUserForSpamWhenPunishmentOptionIsMute(DateTime dateTime, TimeSpan forTime)
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);
            var punishment = new Punishment(PunishmentOption.Mute, dateTime, forTime);

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
