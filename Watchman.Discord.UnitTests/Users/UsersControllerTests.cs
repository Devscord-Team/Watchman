using AutoFixture.NUnit3;
using Devscord.DiscordFramework.Commands.Responses;
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
using Watchman.Cqrs;
using Watchman.Discord.Areas.Users.BotCommands;
using Watchman.Discord.UnitTests.TestObjectFactories;
using Watchman.DomainModel.DiscordServer;
using Watchman.DomainModel.DiscordServer.Queries;
using Devscord.DiscordFramework.Commons.Exceptions;

namespace Watchman.Discord.UnitTests.Users
{
    internal class UsersControllerTests
    {
        private readonly TestControllersFactory testControllersFactory = new();
        private readonly TestContextsFactory testContextsFactory = new();

        [Test, AutoData]
        public async Task GetAvatar_ShouldDisplayAnotherUserAvatar(AvatarCommand command)
        {
            //Arrange
            var anotherUserContext = testContextsFactory.CreateUserContext(2, "test");
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);

            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var usersServiceMock = new Mock<IUsersService>();
            usersServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()))
                .Returns(Task.FromResult(anotherUserContext));

            var controller = this.testControllersFactory.CreateUsersController(
                usersServiceMock: usersServiceMock,
                messagesServiceFactoryMock: messagesServiceFactoryMock);
               
            //Act
            await controller.GetAvatar(command, contexts);

            //Assert
            usersServiceMock.Verify(x => x.GetUserByIdAsync(contexts.Server, command.User), Times.Once);
            messagesServiceMock.Verify(x => x.SendMessage(anotherUserContext.AvatarUrl, It.IsAny<MessageType>()), Times.Once);
        }

        [Test]
        public async Task GetAvatar_ShouldRespondThatUserHasNoAvatar()
        {
            //Arrange
            var command = new AvatarCommand();
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);

            var messagesServiceMock = new Mock<IMessagesService>();
            messagesServiceMock.Setup(x => x.SendMessage(It.IsAny<string>(), It.IsAny<MessageType>()))
               .Returns<string, MessageType>((a, b) => Task.FromResult(contexts.User.AvatarUrl));
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var usersServiceMock = new Mock<IUsersService>();

            var controller = this.testControllersFactory.CreateUsersController(
                usersServiceMock: usersServiceMock,
                messagesServiceFactoryMock: messagesServiceFactoryMock);

            //Act
            await controller.GetAvatar(command, contexts);

            //Assert
            usersServiceMock.Verify(x => x.GetUserByIdAsync(contexts.Server, command.User), Times.Never);
            messagesServiceMock.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<MessageType>()), Times.Never);
            messagesServiceMock.Verify(x => x.SendResponse(It.IsAny<Func<IResponsesService, string>>()), Times.Once);
        }

        [Test]
        public async Task GetAvatar_ShouldDisplayUserOwnAvatar()
        {
            //Arrange
            var command = new AvatarCommand();
            var contexts = testContextsFactory.CreateContexts(1, 1, 1, "test");

            var messagesServiceMock = new Mock<IMessagesService>();               
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var usersServiceMock = new Mock<IUsersService>();

            var controller = this.testControllersFactory.CreateUsersController(
                usersServiceMock: usersServiceMock,
                messagesServiceFactoryMock: messagesServiceFactoryMock);

            //Act
            await controller.GetAvatar(command, contexts);

            //Assert
            usersServiceMock.Verify(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()), Times.Never);
            messagesServiceMock.Verify(x => x.SendMessage(contexts.User.AvatarUrl, It.IsAny<MessageType>()), Times.Once);
            messagesServiceMock.Verify(x => x.SendResponse(It.IsAny<Func<IResponsesService, string>>()), Times.Never);
        }
        [Test]
        public async Task AddRole_ShouldAddRole()
        {
            //Arrange
            AddRoleCommand command = new AddRoleCommand();
            command.Roles = new List<string>();
            var contexts = testContextsFactory.CreateContexts(1, 1, 1, "test");
            var safeRole = new SafeRole(1ul, 3ul);
            var safeRoles = new List<SafeRole>();
            safeRoles.Add(safeRole);

            var rolesServiceMock = new Mock<Areas.Users.Services.IRolesService>();
            var queryBusMock = new Mock<IQueryBus>();
            queryBusMock.Setup(x => x.Execute(It.IsAny<GetDiscordServerSafeRolesQuery>()))
                .Returns(new GetDiscordServerSafeRolesQueryResult(safeRoles));

            var controller = this.testControllersFactory.CreateUsersController(
                queryBusMock: queryBusMock,
                rolesServiceMock: rolesServiceMock);
            
            //Act
            await controller.AddRole(command, contexts);

            //Assert
            queryBusMock.Verify(x => x.Execute(It.IsAny<GetDiscordServerSafeRolesQuery>()), Times.Once);
            rolesServiceMock.Verify(x => x.AddRoleToUser(It.IsAny<IEnumerable<SafeRole>>(), It.IsAny<Contexts>(), It.IsAny<List<string>>()), Times.Once);
        }
        [Test]
        public void AddRole_ShouldThrowException()
        {
            //Arrange
            AddRoleCommand command = new AddRoleCommand();
            command.Roles = new List<string>() { "test", "test", "test", "test", "test", "test" };
            var contexts = testContextsFactory.CreateContexts(1, 1, 1, "test");

            var rolesServiceMock = new Mock<Areas.Users.Services.IRolesService>();
            var queryBusMock = new Mock<IQueryBus>();

            var controller = this.testControllersFactory.CreateUsersController(
                queryBusMock: queryBusMock,
                rolesServiceMock: rolesServiceMock);

            //Act
            Assert.ThrowsAsync<InvalidArgumentsException>(async () => await controller.AddRole(command, contexts));

            //Assert
            queryBusMock.Verify(x => x.Execute(It.IsAny<GetDiscordServerSafeRolesQuery>()), Times.Never);
            rolesServiceMock.Verify(x => x.AddRoleToUser(It.IsAny<IEnumerable<SafeRole>>(), It.IsAny<Contexts>(), It.IsAny<List<string>>()), Times.Never);
        }

        [Test]
        public async Task RemoveRole_ShouldRemoveRole()
        {
            //Arrange
            RemoveRoleCommand command = new RemoveRoleCommand();
            command.Roles = new List<string>();
            var contexts = testContextsFactory.CreateContexts(1, 1, 1, "test");
            var safeRole = new SafeRole(1ul, 3ul);
            var safeRoles = new List<SafeRole>();
            safeRoles.Add(safeRole);

            var rolesServiceMock = new Mock<Areas.Users.Services.IRolesService>();
            var queryBusMock = new Mock<IQueryBus>();
            queryBusMock.Setup(x => x.Execute(It.IsAny<GetDiscordServerSafeRolesQuery>()))
                .Returns(new GetDiscordServerSafeRolesQueryResult(safeRoles));

            var controller = this.testControllersFactory.CreateUsersController(
                queryBusMock: queryBusMock,
                rolesServiceMock: rolesServiceMock);

            //Act
            await controller.RemoveRole(command, contexts);

            //Assert
            queryBusMock.Verify(x => x.Execute(It.IsAny<GetDiscordServerSafeRolesQuery>()), Times.Once);
            rolesServiceMock.Verify(x => x.DeleteRoleFromUser(It.IsAny<IEnumerable<SafeRole>>(), It.IsAny<Contexts>(), It.IsAny<List<string>>()), Times.Once);
        }
        [Test]
        public void RemoveRole_ShouldThrowException()
        {
            //Arrange
            RemoveRoleCommand command = new RemoveRoleCommand();
            command.Roles = new List<string>() { "test", "test", "test", "test", "test", "test" };
            var contexts = testContextsFactory.CreateContexts(1, 1, 1, "test");

            var rolesServiceMock = new Mock<Areas.Users.Services.IRolesService>();
            var queryBusMock = new Mock<IQueryBus>();

            var controller = this.testControllersFactory.CreateUsersController(
                queryBusMock: queryBusMock,
                rolesServiceMock: rolesServiceMock);

            //Act
            Assert.ThrowsAsync<InvalidArgumentsException>(async () => await controller.RemoveRole(command, contexts));

            //Assert
            queryBusMock.Verify(x => x.Execute(It.IsAny<GetDiscordServerSafeRolesQuery>()), Times.Never);
            rolesServiceMock.Verify(x => x.DeleteRoleFromUser(It.IsAny<IEnumerable<SafeRole>>(), It.IsAny<Contexts>(), It.IsAny<List<string>>()), Times.Never);
        }

    }
}
