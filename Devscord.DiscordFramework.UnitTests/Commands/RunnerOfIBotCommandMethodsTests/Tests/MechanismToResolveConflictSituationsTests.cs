using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Controllers;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using NUnit.Framework;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Tests
{
    // "Conflict situations" means situations in which a given command matches default and custom version of a bot command at the same time. During these situations, "mechanism to resolve conflict situations" is runned. 
    // It means a method which check what arguments are known for a bot command. Thanks to the mechanism, it is possible to determine if the command is in the default or custom version.
    // It matters for command correctness because setting of version is needed to determine how to parse the commands
    [TestFixture]
    class MechanismToResolveConflictSituationsTests
    {
        private readonly GetterOfThingsAboutBotCommand _getterOfThings;
        private readonly TestController _controller;
        private readonly CommandParser _commandParser;

        public MechanismToResolveConflictSituationsTests()
        {
            this._getterOfThings = new GetterOfThingsAboutBotCommand();
            this._controller = new TestController();
            this._commandParser = new CommandParser();
        }

        [Test]
        public async Task ShouldGiveCorrectArgs_WhenGivenCommandIsActuallyDefaultCommandButMatchesCustomVersionOfCommand()
        {
            // Arrange:
            var customCommands = this._getterOfThings.CreateCustomCommandFor<SomeDefaultCommand>(@"-somedefault\s*(?<Time>.*)\s*(?<User>.*)\s*(?<List>.*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._controller);
            var testCommand = "-somedefault -time 4m -user <@123> -list abc qwerty";
            var request = this._commandParser.Parse(testCommand, DateTime.Now);

            // Act:
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert:
            var args = this._controller.DefaultCommandArgs;
            Assert.That(args.Time, Is.EqualTo(TimeSpan.FromMinutes(4)));
            Assert.That(args.User, Is.EqualTo(123UL));
            Assert.That(args.List, Is.EqualTo(new List<string> { "abc", "qwerty" }));
        }

        [Test]
        public async Task ShouldGiveRightArgs_WhenCommandIsCustomCommandButMatchesDefaultVersionOfCommand()
        {
            // Arrange:
            var customCommands = this._getterOfThings.CreateCustomCommandFor<SomeCustomCommand>(@"-somecustom\s*-list\s*(?<List>.*)\s*(?<Bool>-b|-bool)\s*(?<Number>.*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._controller);
            var testCommand = "-somecustom -list 123 456 789 -b 123";
            var request = this._commandParser.Parse(testCommand, DateTime.Now);

            // Act:
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert:
            var args = this._controller.CustomCommandArgs;
            Assert.That(args.List, Is.EqualTo(new List<string> { "123", "456", "789" }));
            Assert.That(args.Bool, Is.EqualTo(true));
            Assert.That(args.Number, Is.EqualTo(123));
        }
    }
}
