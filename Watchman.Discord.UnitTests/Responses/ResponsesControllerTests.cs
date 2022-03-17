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

namespace Watchman.Discord.UnitTests.Responses
{
    internal class ResponsesControllerTests
    {
        private readonly TestControllersFactory testControllersFactory = new();
        private readonly TestContextsFactory testContextsFactory = new();
        [Test, AutoData]
        public async Task ShouldUpdateResponse(UpdateResponseCommand command, Contexts contexts)
        {
            var userContext = testContextsFactory.CreateUserContext(1);

            var watchmanResponsesServiceMock = new Mock<Areas.Responses.Services.IResponsesService>();
            watchmanResponsesServiceMock.Setup(x => x.GetResponseByOnEvent(It.IsAny<string>(), It.IsAny<ulong>()))
                .Returns((Task<DomainModel.Responses.Response>)watchmanResponsesServiceMock.Object);
            watchmanResponsesServiceMock.Setup(x => x.UpdateResponse(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            var devscordResponsesServiceMock = new Mock<Devscord.DiscordFramework.Commands.Responses.IResponsesService>();
            //devscordResponsesServiceMock.Setup(x => x.ResponseHasBeenUpdated(It.IsAny<Contexts>(), It.IsAny<UpdateResponseCommand>(), It.IsAny<DomainResponse>))
            var messagesServiceMock = new Mock<IMessagesService>();
            messagesServiceMock.Setup(x => x.SendResponse((Func<Devscord.DiscordFramework.Commands.Responses.IResponsesService, string>)It.IsAny<Func<IResponsesService, string>>()))
                .Returns(Task.CompletedTask);

            

            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);

            var controller = this.testControllersFactory.Cre




        }
    }
}
