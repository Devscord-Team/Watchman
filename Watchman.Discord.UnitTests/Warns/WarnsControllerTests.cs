using AutoFixture.NUnit3;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands.Responses;
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
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Users.BotCommands;
using FluentAssertions;
using Watchman.Discord.UnitTests.TestObjectFactories;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Watchman.Discord.Areas.Users.BotCommands.Warns;
using Devscord.DiscordFramework.Commons.Exceptions;

namespace Watchman.Discord.UnitTests.Warns
{
    internal class WarnsControllerTests
    {
        private readonly TestControllersFactory testControllersFactory = new();
        private readonly TestContextsFactory testContextsFactory = new();

        [Test, AutoData]
        public async Task AddWarn_ShouldAddWarnToUser(AddWarnCommand command)
        {
            //Arrange
            var userContext = testContextsFactory.CreateUserContext(1);
            var contexts = testContextsFactory.CreateContexts(1,1,1);

            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var warnsServiceMock = new Mock<IWarnsService>();
            var usersServiceMock = new Mock<IUsersService>();
            usersServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()))
                .Returns<DiscordServerContext, ulong>((a, b) => Task.FromResult(userContext));

            var controller = this.testControllersFactory.CreateWarnsController(
                messagesServiceFactoryMock: messagesServiceFactoryMock,
                warnsServiceMock: warnsServiceMock,
                usersServiceMock: usersServiceMock);
            
            //Act
            await controller.AddWarn(command, contexts);

            //Assert
            messagesServiceFactoryMock.Verify(x => x.Create(contexts), Times.Once);
            warnsServiceMock.Verify(x => x.AddWarnToUser(command, contexts, userContext), Times.Once);
            usersServiceMock.Verify(x => x.GetUserByIdAsync(contexts.Server, command.User), Times.Once);
        }

        [Test]
        public async Task GetWarn_ShouldReturnWarnsOfCurrentUserIfNotMentioned()
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);
            var command = new WarnsCommand();

            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var warnsServiceMock = new Mock<IWarnsService>();
            warnsServiceMock.Setup(x => x.GetWarns(It.IsAny<UserContext>(), It.IsAny<ulong>()))
                .Returns<UserContext, ulong>((a, b) => new List<KeyValuePair<string, string>>());

            var controller = this.testControllersFactory.CreateWarnsController(
                messagesServiceFactoryMock: messagesServiceFactoryMock,
                warnsServiceMock: warnsServiceMock);

            //Act
            await controller.GetWarns(command, contexts);

            //Assert
            messagesServiceFactoryMock.Verify(x => x.Create(contexts), Times.Once);
            warnsServiceMock.Verify(x => x.GetWarns(contexts.User, contexts.Server.Id), Times.Once);
            messagesServiceMock.Verify(x => x.SendEmbedMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Once);

        }

        [Test]
        [TestCase(5u)]
        public async Task GetWarn_ShouldReturnWarnsOfMentionedUser(ulong mentionedUserId)
        {
            //Arrange
            var mentionedUserContext = testContextsFactory.CreateUserContext(mentionedUserId);
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);
            var command = new WarnsCommand() { User = mentionedUserId };

            var messagesServiceMock = new Mock<IMessagesService>();
            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            messagesServiceFactoryMock.Setup(x => x.Create(It.IsAny<Contexts>()))
                .Returns(messagesServiceMock.Object);
            var warnsServiceMock = new Mock<IWarnsService>();
            warnsServiceMock.Setup(x => x.GetWarns(It.IsAny<UserContext>(), It.IsAny<ulong>()))
                .Returns<UserContext, ulong>((a, b) => new List<KeyValuePair<string, string>>());
            var usersServiceMock = new Mock<IUsersService>();
            usersServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()))
                .Returns(Task.FromResult(mentionedUserContext));
            var controller = this.testControllersFactory.CreateWarnsController(
                messagesServiceFactoryMock: messagesServiceFactoryMock,
                warnsServiceMock: warnsServiceMock,
                usersServiceMock: usersServiceMock);

            //Act
            await controller.GetWarns(command, contexts);

            //Assert
            messagesServiceFactoryMock.Verify(x => x.Create(contexts), Times.Once);
            usersServiceMock.Verify(x => x.GetUserByIdAsync(contexts.Server, mentionedUserId));
            warnsServiceMock.Verify(x => x.GetWarns(mentionedUserContext, contexts.Server.Id), Times.Once);
            messagesServiceMock.Verify(x => x.SendEmbedMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Once);
        }

        [Test]
        public async Task GetWarn_ShouldThrowExceptionIfUserNotFound()
        {
            //Arrange
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);
            var command = new WarnsCommand() { User = 2 };

            var controller = this.testControllersFactory.CreateWarnsController();
            //Act
            Assert.ThrowsAsync<UserNotFoundException>(() => controller.GetWarns(command, contexts));
        }
    }
}