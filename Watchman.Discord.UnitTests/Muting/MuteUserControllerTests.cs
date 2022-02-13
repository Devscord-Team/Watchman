﻿using AutoFixture;
using AutoFixture.NUnit3;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.BotCommands;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Protection.Services.Commands;
using Watchman.Discord.UnitTests.TestObjectFactories;
using Watchman.DomainModel.Protection.Mutes;

namespace Watchman.Discord.UnitTests.Muting
{
    [TestFixture]
    internal class MuteUserControllerTests
    {
        private readonly TestControllersFactory testControllersFactory = new();

        //what if userContext and userBotContext has same ID? chance is low, but never zero
        [Test, AutoData]
        public async Task MuteUser_ShouldMuteUser(MuteCommand command, UserContext userContext, UserContext userBotContext, DiscordServerContext serverContext, ChannelContext channelContext)
        {
            //Arrange
            var contexts = new Contexts(serverContext, channelContext, userContext);

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
        public async Task MuteUser_ShouldUnmuteUserInFuture(MuteCommand command, UserContext userContext, UserContext userBotContext, DiscordServerContext serverContext, ChannelContext channelContext)
        {
            //Arrange
            var contexts = new Contexts(serverContext, channelContext, userContext);

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
        public void MuteUser_ShouldThrowExceptionIfUserNotExist(MuteCommand command, Contexts contexts)
        {
            //Arrange
            var commandBusMock = new Mock<ICommandBus>();
            var usersServiceMock = new Mock<IUsersService>();
            usersServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()))
                .Returns<DiscordServerContext, ulong>((a, b) => null);
            var controller = this.testControllersFactory.CreateMuteUserController(commandBusMock: commandBusMock, usersServiceMock: usersServiceMock);

            //Act
            Assert.ThrowsAsync<UserNotFoundException>(() => controller.MuteUser(command, contexts));

            //Assert
            commandBusMock.Verify(x => x.ExecuteAsync(It.IsAny<MuteUserOrOverwriteCommand>()), Times.Never);
        }

        [Test, AutoData]
        public void MuteUser_ShouldThrowExceptionIfUserTriedToMuteWatchman(MuteCommand command, UserContext userBotContext, DiscordServerContext serverContext, ChannelContext channelContext)
        {
            //Arrange
            var contexts = new Contexts(serverContext, channelContext, userBotContext);

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
    }
}