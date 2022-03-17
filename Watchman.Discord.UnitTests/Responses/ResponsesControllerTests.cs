using AutoFixture.NUnit3;
using Devscord.DiscordFramework.Middlewares.Contexts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Responses.BotCommands;
using Watchman.Discord.UnitTests.TestObjectFactories;

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
            

        }
    }
}
