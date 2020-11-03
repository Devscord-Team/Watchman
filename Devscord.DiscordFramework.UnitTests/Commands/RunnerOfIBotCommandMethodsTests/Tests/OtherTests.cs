using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Controllers;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using NUnit.Framework;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Tests
{
    [TestFixture]
    class OtherTests
    {
        private readonly GetterOfThingsAboutBotCommand _getterOfThings;
        private readonly TestController _controller;
        private readonly CommandParser _commandParser;

        public OtherTests()
        {
            this._getterOfThings = new GetterOfThingsAboutBotCommand();
            this._controller = new TestController();
            this._commandParser = new CommandParser();
        }

        [Test]
        public void ShouldThrowException_WhenUserIsNotAdminAndUserUsesCommandOnlyForAdmin()
        {
            // Arrange:
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts(isOwnerOrAdmin: false);
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._controller);
            var request = this._commandParser.Parse("-optionalArgs", DateTime.Now);

            // Act:
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert:
            Assert.ThrowsAsync<NotAdminPermissionsException>(RunMethodsFunc);
        }

        [Test]
        public void ShouldDoesNotThrowException_WhenAdminUsesCommandOnlyForAdmin()
        {
            // Arrange:
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts(isOwnerOrAdmin: true);
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._controller);
            var request = this._commandParser.Parse("-optionalArgs", DateTime.Now);

            // Act:
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert:
            Assert.DoesNotThrowAsync(RunMethodsFunc);
        }

        [Test]
        [TestCase("-text")]
        [TestCase("-usermention   -TestUserMention")]
        [TestCase("-somedefault -time 4h -list sth")]
        [TestCase("-custom_text    ")]
        [TestCase("-custom_user")]
        [TestCase("-somedefault -t  -l sth -u <@123>")]
        public void ShouldThrowException_WhenCommandIsGivenWithoutAllRequiredArgs(string message)
        {
            // Arrange:
            var customCommands = this._getterOfThings.CreateCustomCommands(new (Type type, string regexInText)[] 
            { 
                (typeof(TextCommand), @"-custom_text\s*(?<TestText>.*)"),
                (typeof(UserMentionCommand), @"-custom_user\s*(?<TestUserMention>.*)"),
                (typeof(SomeDefaultCommand), @"-somedefault\s*-t\s*(?<Time>\d+(m|ms|d|s|h))\s*-l\s*(?<List>.+)\s*-u\s*(?<User>.+)"),
            });
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._controller);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act:
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert:
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }

        [Test]
        [TestCase("-text -testtext qwerty")]
        [TestCase("-usermention -TestUserMention <@123>")]
        [TestCase("-somedefault -time 4h -list sth -user <@123>")]
        [TestCase("-custom_text anything")]
        [TestCase("-custom_user <@123>")]
        [TestCase("-somedefault -t 4h -l sth else -u <@123>")]
        public void ShouldDoesNotThrowException_WhenGivenCommandHasEveryRequiredArg(string message)
        {
            // Arrange:
            var customCommands = this._getterOfThings.CreateCustomCommands(new (Type type, string regexInText)[]
            {
                (typeof(TextCommand), @"-custom_text\s*(?<TestText>.*)"),
                (typeof(UserMentionCommand), @"-custom_user\s*(?<TestUserMention>.*)"),
                (typeof(SomeDefaultCommand), @"-somedefault\s*-t\s*(?<Time>\d+(m|ms|d|s|h))\s*-l\s*(?<List>.+)\s*-u\s*(?<User>.+)"),
            });
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._controller);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act:
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert:
            Assert.DoesNotThrowAsync(RunMethodsFunc);
        }
    }
}
