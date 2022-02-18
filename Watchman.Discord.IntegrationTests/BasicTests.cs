using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.IntegrationTests.TestEnvironment;

namespace Watchman.Discord.IntegrationTests
{
    internal class BasicTests
    {
        //todo improve, there should be bot per group of tests instead bot per test
        private TestWatchmanBotFactory testWatchmanBotFactory = new();

        [Test]
        public void NormalMessageShouldNotThrowException()
        {
            //Arrange
            var commandsRunner = this.testWatchmanBotFactory.CreateCommandsRunner();

            //Act
            Assert.DoesNotThrowAsync(() => commandsRunner.SendMessage("Health check"));
        }
    }
}
