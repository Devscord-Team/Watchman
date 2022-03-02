using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Users.BotCommands;
using Watchman.Discord.UnitTests.TestObjectFactories;

namespace Watchman.Discord.UnitTests.Users
{
    internal class UsersControllerTests
    {
        private readonly TestControllersFactory testControllersFactory = new();
        private readonly TestContextsFactory testContextsFactory = new();

        [Test]
        public async Task GetAvatar_ShouldDisplayUserAvatar(AvatarCommand command)
        {
            //Arrange
            var userContext = testContextsFactory.CreateUserContext(1);
            var contexts = testContextsFactory.CreateContexts(1, 1, 1);

            var messagesServiceMock = new Mock<IMessagesService>();
            var usersServiceMock = new Mock<IUsersService>();
            usersServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<DiscordServerContext>(), It.IsAny<ulong>()))
                .Returns<DiscordServerContext, ulong>((a, b) => Task.FromResult(userContext));

            
        }

    }
}
