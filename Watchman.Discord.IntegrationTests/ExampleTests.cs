using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.IntegrationTests.TestEnvironment;

namespace Watchman.Discord.IntegrationTests
{
    internal class ExampleTests
    {
        private TestWatchmanBotFactory testWatchmanBotFactory = new();

        [Test]
        public async Task Example()
        {
            //Arrange
            var commandsRunner = this.testWatchmanBotFactory.CreateCommandsRunner();

            //Act
            await commandsRunner.SendMessage("test");
        }
    }
}
