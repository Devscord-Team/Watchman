using Devscord.DiscordFramework.Commons.Exceptions;
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
        public void Message_ShouldNotThrowException()
        {
            //Arrange
            var commandsRunner = this.testWatchmanBotFactory.CreateCommandsRunner();

            //Act
            Assert.DoesNotThrowAsync(() => commandsRunner.SendMessage("Health check"));
        }

        [Test]
        public void UnknownCommand_ShouldNotThrowException()
        {
            //Arrange
            var commandsRunner = this.testWatchmanBotFactory.CreateCommandsRunner();

            //Act
            Assert.DoesNotThrowAsync(() => commandsRunner.SendMessage("-not exist command"));
        }

        [Test]
        public void InvalidCommand_ShouldThrowException()
        {
            //Arrange
            var commandsRunner = this.testWatchmanBotFactory.CreateCommandsRunner();

            //Act
            Assert.ThrowsAsync<InvalidArgumentsException>(() => commandsRunner.SendMessage("-setrole")); //without params
        }

        [Test]
        [TestCase("-dontask")]
        [TestCase("-google -search test")]
        [TestCase("-marchew")]
        [TestCase("-maruda")]
        [TestCase("-nohello")]
        public void UselessFeatures_ShouldNotThrowException(string commandName)
        {
            //Arrange
            var commandsRunner = this.testWatchmanBotFactory.CreateCommandsRunner();

            //Act
            Assert.DoesNotThrowAsync(() => commandsRunner.SendMessage(commandName));
        }

        [Test]
        public void Help_ShouldNotThrowException()
        {
            //Arrange
            var commandsRunner = this.testWatchmanBotFactory.CreateCommandsRunner();

            //Act
            Assert.DoesNotThrowAsync(() => commandsRunner.SendMessage("-help"));
        }

        [Test]
        [TestCase("-mute -user <@123> -time 10s -reason test")]
        [TestCase("-unmute -user <@123>")]
        [TestCase("-trustedroles")]
        public void AdminCommands_IfNotOwnerShouldThrowException(string command)
        {
            //Arrange
            var commandsRunner = this.testWatchmanBotFactory.CreateCommandsRunner();

            //Act
            Assert.ThrowsAsync<NotAdminPermissionsException>(() => commandsRunner.SendMessage(command));
        }
    }
}
