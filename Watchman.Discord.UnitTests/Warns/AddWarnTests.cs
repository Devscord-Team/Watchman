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

namespace Watchman.Discord.UnitTests.Warns
{

    internal class AddWarnTests
    {
        private readonly TestControllersFactory testControllersFactory = new();
        private readonly TestContextsFactory testContextsFactory = new();




        [Test, AutoData]
        public async Task ShouldAddWarnToUser(AddWarnCommand command, Contexts contexts)
        {
            //Arrange
            var userContext = testContextsFactory.CreateUserContext(1);
            var userBotContext = testContextsFactory.CreateUserContext(2);

            

            var messagesServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            var warnsServiceMock = new Mock<IWarnsService>();
            var usersServiceMock = new Mock<IUsersService>();

            usersServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()))
                .Returns<DiscordServerContext, ulong>((a, b) => Task.FromResult(userBotContext));
            usersServiceMock.Setup(x => x.GetBot())
                .Returns(userBotContext);
            contexts.SetContext(userContext);


            var controller = this.testControllersFactory.CreateWarnsController(
                messagesServiceFactoryMock: messagesServiceFactoryMock,
                warnsServiceMock: warnsServiceMock,
                usersServiceMock: usersServiceMock);

            //Asset
              await controller.AddWarn(command, contexts);

            //Assert
            messagesServiceFactoryMock.Verify(x => x.Create(contexts), Times.Once);
            warnsServiceMock.Verify(x => x.AddWarnToUser(command, contexts, userContext), Times.Once);




            //var messageService = messageServiceFactoryMock.Create(contexts);
            //var mentionedUser = await this._usersService.GetUserByIdAsync(contexts.Server, command.User);
            //await this._warnService.AddWarnToUser(command, contexts, mentionedUser);
            //await messageService.SendResponse(x => x.UserHasBeenWarned(contexts.User.Name, mentionedUser.Name, command.Reason));
        }

    }
}