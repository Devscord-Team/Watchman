using AutoFixture;
using AutoFixture.NUnit3;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Commons.Exceptions;
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
using Watchman.Cqrs;
using Watchman.Discord.Areas.Muting.BotCommands;
using Watchman.Discord.Areas.Muting.Services;
using Watchman.Discord.Areas.Muting.Services.Commands;
using Watchman.Discord.UnitTests.TestObjectFactories;
using Watchman.DomainModel.Muting;

namespace Watchman.Discord.UnitTests.Muting
{
    [TestFixture]
    internal class MuteUserControllerTests
    {
        private readonly TestControllersFactory testControllersFactory = new();
        private readonly TestContextsFactory testContextsFactory = new();

        //what if userContext and userBotContext has same ID? chance is low, but never zero
        [Test, AutoData]
        public async Task MuteUser_ShouldMuteUser(MuteCommand command)
        {
            //Arrange
            var userContext = testContextsFactory.CreateUserContext(1);
            var userBotContext = testContextsFactory.CreateUserContext(2);
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);

            var commandBusMock = new Mock<ICommandBus>();
            var usersServiceMock = new Mock<IUsersService>();
            var unmutingServiceMock = new Mock<IUnmutingService>();

            usersServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()))
                .Returns<DiscordServerContext, ulong>((a, b) => Task.FromResult(userContext));
            usersServiceMock.Setup(x => x.GetBot())
                .Returns(userBotContext);

            var controller = this.testControllersFactory.CreateMuteUserController(
                commandBusMock: commandBusMock,
                usersServiceMock: usersServiceMock,
                unmutingServiceMock: unmutingServiceMock);

            //Act
            await controller.MuteUser(command, contexts);

            //Assert
            commandBusMock.Verify(x => x.ExecuteAsync(It.IsAny<MuteUserOrOverwriteCommand>()), Times.Once);
        }

        [Test, AutoData]
        public async Task MuteUser_ShouldUnmuteUserInFuture(MuteCommand command)
        {
            //Arrange
            var userContext = testContextsFactory.CreateUserContext(1);
            var userBotContext = testContextsFactory.CreateUserContext(2);
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);

            var commandBusMock = new Mock<ICommandBus>();
            var usersServiceMock = new Mock<IUsersService>();
            var unmutingServiceMock = new Mock<IUnmutingService>();

            usersServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()))
                .Returns<DiscordServerContext, ulong>((a, b) => Task.FromResult(userContext));
            usersServiceMock.Setup(x => x.GetBot())
                .Returns(userBotContext);

            var controller = this.testControllersFactory.CreateMuteUserController(
                commandBusMock: commandBusMock, 
                usersServiceMock: usersServiceMock, 
                unmutingServiceMock: unmutingServiceMock);

            //Act
            await controller.MuteUser(command, contexts);

            //Assert
            commandBusMock.Verify(x => x.ExecuteAsync(It.IsAny<MuteUserOrOverwriteCommand>()), Times.Once);
            unmutingServiceMock.Verify(x => x.UnmuteInFuture(contexts, It.IsAny<MuteEvent>(), userContext), Times.Once);
        }

        [Test, AutoData]
        public void MuteUser_ShouldThrowExceptionIfUserNotExist(MuteCommand command)
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);

            var commandBusMock = new Mock<ICommandBus>();
            var usersServiceMock = new Mock<IUsersService>();
            usersServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()))
                .Returns<DiscordServerContext, ulong>((a, b) => Task.FromResult(null as UserContext));
            var controller = this.testControllersFactory.CreateMuteUserController(commandBusMock: commandBusMock, usersServiceMock: usersServiceMock);

            //Act
            Assert.ThrowsAsync<UserNotFoundException>(() => controller.MuteUser(command, contexts));

            //Assert
            commandBusMock.Verify(x => x.ExecuteAsync(It.IsAny<MuteUserOrOverwriteCommand>()), Times.Never);
        }

        [Test, AutoData]
        public void MuteUser_ShouldThrowExceptionIfUserTriedToMuteWatchman(MuteCommand command)
        {
            //Arrange
            var userContext = testContextsFactory.CreateUserContext(1);
            var userBotContext = testContextsFactory.CreateUserContext(2);
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);

            var commandBusMock = new Mock<ICommandBus>();
            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()))
                .Returns<DiscordServerContext, ulong>((a, b) => Task.FromResult(userBotContext));
            usersServiceMock.Setup(x => x.GetBot())
                .Returns(userBotContext);

            var controller = this.testControllersFactory.CreateMuteUserController(commandBusMock: commandBusMock, usersServiceMock: usersServiceMock);

            //Act
            Assert.ThrowsAsync<UserNotFoundException>(() => controller.MuteUser(command, contexts));

            //Assert
            commandBusMock.Verify(x => x.ExecuteAsync(It.IsAny<MuteUserOrOverwriteCommand>()), Times.Never);
        }

        [Test, AutoData]
        public async Task UnmuteUser_ShouldUnmuteUser(UnmuteCommand command)
        {
            //Arrange
            var userContext = testContextsFactory.CreateUserContext(1);
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);

            var commandBusMock = new Mock<ICommandBus>();
            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()))
                .Returns<DiscordServerContext, ulong>((a, b) => Task.FromResult(userContext));

            var controller = this.testControllersFactory.CreateMuteUserController(commandBusMock: commandBusMock, usersServiceMock: usersServiceMock);

            //Act
            await controller.UnmuteUser(command, contexts);

            //Assert
            commandBusMock.Verify(x => x.ExecuteAsync(It.IsAny<UnmuteNowCommand>()), Times.Once);
        }

        [Test, AutoData]
        public void UnmuteUser_ShouldThrowExceptionIfUserNotExist(UnmuteCommand command)
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);

            var commandBusMock = new Mock<ICommandBus>();
            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()))
                .Returns<DiscordServerContext, ulong>((a, b) => Task.FromResult(null as UserContext));

            var controller = this.testControllersFactory.CreateMuteUserController(commandBusMock: commandBusMock, usersServiceMock: usersServiceMock);

            //Act
            Assert.ThrowsAsync<UserNotFoundException>(() => controller.UnmuteUser(command, contexts));

            //Assert
            commandBusMock.Verify(x => x.ExecuteAsync(It.IsAny<UnmuteNowCommand>()), Times.Never);
        }

        [Test, AutoData]
        public async Task GetMutedUsers_ShouldSendMutedUsersDirectMessage(MutedUsersCommand command)
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);
            var messagesServiceMock = new Mock<IMessagesService>();

            var commandBusMock = new Mock<ICommandBus>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();

            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);

            var controller = this.testControllersFactory.CreateMuteUserController(commandBusMock: commandBusMock, messagesServiceFactoryMock: messagesServiceFactoryMock);

            //Act
            await controller.GetMutedUsers(command, contexts);

            //Assert
            commandBusMock.Verify(x => x.ExecuteAsync(It.IsAny<SendMutedUsersDirectMessageCommand>()), Times.Once);
        }

        [Test, AutoData]
        public async Task GetMutedUsers_ShouldSendMutedUsersListSentResponse(MutedUsersCommand command)
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);
            var messagesServiceMock = new Mock<IMessagesService>();

            var commandBusMock = new Mock<ICommandBus>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();

            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);

            var controller = this.testControllersFactory.CreateMuteUserController(commandBusMock: commandBusMock, messagesServiceFactoryMock: messagesServiceFactoryMock);

            //Act
            await controller.GetMutedUsers(command, contexts);

            //Assert
            messagesServiceMock.Verify(x => x.SendResponse(It.IsAny<Func<IResponsesService, string>>()), Times.Once);
        }
    }
}
