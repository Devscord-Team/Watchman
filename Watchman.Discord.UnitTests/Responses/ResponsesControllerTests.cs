using AutoFixture.NUnit3;
using Watchman.Discord.Areas.Responses.Services;
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
using Watchman.Discord.Areas.Responses.BotCommands;
using Watchman.Discord.UnitTests.TestObjectFactories;
using Devscord.DiscordFramework.Commands.Responses;
using IResponsesService = Watchman.Discord.Areas.Responses.Services.IResponsesService;

namespace Watchman.Discord.UnitTests.Responses
{
    internal class ResponsesControllerTests
    {
        private readonly TestControllersFactory testControllersFactory = new();
        private readonly TestContextsFactory testContextsFactory = new();
        [Test, AutoData]
        public async Task UpdateResponse_ShouldUpdateResponse(UpdateResponseCommand command)
        {
            //Arrange
            var expectedResponse = new DomainModel.Responses.Response("test", "test", (ulong)43, new string[] { "test" });
            var serverId = 5u;
            var contexts = this.testContextsFactory.CreateContexts(serverId, 1, 1);
            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var responsesServiceMock = new Mock<IResponsesService>();
            responsesServiceMock.Setup(x => x.GetResponseByOnEvent(It.IsAny<string>(), It.IsAny<ulong>()))
                .Returns(Task.FromResult(expectedResponse));

            var controller = this.testControllersFactory.CreateResponsesController(
                messagesServiceFactoryMock: messagesServiceFactoryMock,
                responsesServiceMock: responsesServiceMock);

            //Act
            await controller.UpdateResponse(command, contexts);

            //Assert
            messagesServiceFactoryMock.Verify(x => x.Create(contexts), Times.Once);
            responsesServiceMock.Verify(x => x.GetResponseByOnEvent(It.IsAny<string>(), serverId), Times.Once);
            responsesServiceMock.Verify(x => x.GetResponseByOnEvent(It.IsAny<string>(), DomainModel.Responses.Response.DEFAULT_SERVER_ID), Times.Once);
            responsesServiceMock.Verify(x => x.RemoveResponse(It.IsAny<string>(), It.Is<ulong>(a => a != contexts.Server.Id)), Times.Never);
            responsesServiceMock.Verify(x => x.UpdateResponse(expectedResponse.Id, It.IsAny<string>()), Times.Once);
        }

        [Test, AutoData]
        public async Task UpdateResponse_ShouldNotFindAnyResponse(UpdateResponseCommand command)
        {
            //Arrange
            var contexts = this.testContextsFactory.CreateContexts(5, 1, 1);

            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var responsesServiceMock = new Mock<IResponsesService>();

            var controller = this.testControllersFactory.CreateResponsesController(
                messagesServiceFactoryMock: messagesServiceFactoryMock,
                responsesServiceMock: responsesServiceMock);

            //Act
            await controller.UpdateResponse(command, contexts);

            //Assert
            messagesServiceFactoryMock.Verify(x => x.Create(contexts), Times.Once);
            responsesServiceMock.Verify(x => x.RemoveResponse(It.IsAny<string>(), It.IsAny<ulong>()), Times.Never);
        }

        [Test, AutoData]
        public async Task UpdateResponse_ShouldRemoveServerResponseIfItsSameAsDefault(UpdateResponseCommand command)
        {
            //Arrange
            var contexts = this.testContextsFactory.CreateContexts(5, 1, 1);
            var expectedResponse = new DomainModel.Responses.Response("test", "test", (ulong)43, new string[] { "test" });

            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var responsesServiceMock = new Mock<IResponsesService>();
            responsesServiceMock.Setup(x => x.GetResponseByOnEvent(It.IsAny<string>(), It.IsAny<ulong>()))
               .Returns(Task.FromResult(new DomainModel.Responses.Response("test", command.Message, 43u, new string[] { "test" })));
            
            var controller = this.testControllersFactory.CreateResponsesController(
                messagesServiceFactoryMock: messagesServiceFactoryMock,
                responsesServiceMock: responsesServiceMock);

            //Act
            await controller.UpdateResponse(command, contexts);

            //Assert
            messagesServiceFactoryMock.Verify(x => x.Create(contexts), Times.Once);
            responsesServiceMock.Verify(x => x.GetResponseByOnEvent(It.IsAny<string>(), DomainModel.Responses.Response.DEFAULT_SERVER_ID), Times.Once);
            responsesServiceMock.Verify(x => x.RemoveResponse(It.IsAny<string>(), It.IsAny<ulong>()), Times.Once);
            responsesServiceMock.Verify(x => x.UpdateResponse(expectedResponse.Id, It.IsAny<string>()), Times.Never);
        }
        [Test]
        public async Task RemoveResponse_ShouldRemoveResponse()
        {
            //Arrange
            var contexts = this.testContextsFactory.CreateContexts(1, 1, 1);

            var serverId = 5ul;
            var command = new RemoveResponseCommand() { OnEvent = "test" };
            var expectedResponse = new DomainModel.Responses.Response("test", "test", (ulong)43, new string[] { "test" });
           
            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var responsesServiceMock = new Mock<IResponsesService>();
            responsesServiceMock.Setup(x => x.GetResponseByOnEvent(It.IsAny<string>(), It.IsAny<ulong>()))
                .Returns(Task.FromResult(expectedResponse));

            var controller = this.testControllersFactory.CreateResponsesController(
                messagesServiceFactoryMock: messagesServiceFactoryMock,
                responsesServiceMock: responsesServiceMock);

            //Act
            await controller.RemoveResponse(command, contexts);

            //Assert
            messagesServiceFactoryMock.Verify(x => x.Create(contexts), Times.Once());
            responsesServiceMock.Verify(x => x.GetResponseByOnEvent(It.IsAny<string>(), It.IsAny<ulong>()), Times.Once);
            responsesServiceMock.Verify(x => x.RemoveResponse(It.IsAny<string>(), It.IsAny<ulong>()), Times.Once);
        }
        [Test]
        public async Task RemoveResponse_ShouldNotRemoveResponse()
        {
            //Arrange
            var contexts = this.testContextsFactory.CreateContexts(5, 1, 1);
            var command = new RemoveResponseCommand() { OnEvent = "test" };

            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var responsesServiceMock = new Mock<IResponsesService>();

            var controller = this.testControllersFactory.CreateResponsesController(
                messagesServiceFactoryMock: messagesServiceFactoryMock,
                responsesServiceMock: responsesServiceMock);

            //Act
            await controller.RemoveResponse(command, contexts);

            //Assert
            messagesServiceFactoryMock.Verify(x => x.Create(contexts), Times.Once);
            responsesServiceMock.Verify(x => x.RemoveResponse(It.IsAny<string>(), It.IsAny<ulong>()), Times.Never);
        }
    }
}   
