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

namespace Watchman.Discord.UnitTests.Warns
{

    internal class AddWarnTests
    {



        [Test, AutoData]
        public async Task ShouldAddWarnToUser(AddWarnCommand command, Contexts contexts)
        {
            var messageServiceFactoryMock = new Mock<IMessagesServiceFactory>();
            var warnServiceMock = new Mock<IWarnsService>();
            var usersServiceMock = new Mock<IUsersService>();

            

            //var messageService = messageServiceFactoryMock.Create(contexts);
            //var mentionedUser = await this._usersService.GetUserByIdAsync(contexts.Server, command.User);
            //await this._warnService.AddWarnToUser(command, contexts, mentionedUser);
            //await messageService.SendResponse(x => x.UserHasBeenWarned(contexts.User.Name, mentionedUser.Name, command.Reason));
        }

    }
}