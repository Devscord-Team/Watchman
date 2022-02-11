using AutoFixture.NUnit3;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Messaging.Administration.BotCommands;
using Watchman.Discord.Areas.Messaging.Administration.Controllers;

namespace Watchman.Discord.UnitTests.Administration
{
    [TestFixture]
    internal class SendControllerTests
    {
        [Test, AutoData]
        public async Task Send_ShouldSendMessage(SendCommand command, Contexts contexts)
        {
            //Arrange
            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock
                .Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);

            var controller = new SendController(messagesServiceFactoryMock.Object);

            //Act
            await controller.Send(command, contexts);

            //Assert
            messagesServiceMock.VerifySet(x => x.ChannelId = command.Channel, Times.Once);
            messagesServiceMock.Verify(x => x.SendMessage(command.Message, It.IsAny<MessageType>()), Times.Once);
        }
    }
}
