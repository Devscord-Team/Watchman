using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Services;
using Moq;
using System;
using System.Threading.Tasks;

namespace Watchman.Discord.UnitTests.TestObjectFactories
{
    // TODO: Use this anywhere it's needed.
    internal class MessagesServiceMockWithResponsesFactory
    {
        public Mock<IMessagesService> Create(Mock<IResponsesService> responsesServiceMock)
        {
            var messagesServiceMock = new Mock<IMessagesService>();
            messagesServiceMock.Setup(x => x.SendResponse(It.IsAny<Func<IResponsesService, string>>()))
                .Callback<Func<IResponsesService, string>>(x => x.Invoke(responsesServiceMock.Object))
                .Returns(Task.CompletedTask);

            return messagesServiceMock;
        }
    }
}
