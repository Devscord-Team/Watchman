using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Controllers;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using NUnit.Framework;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Tests
{
    [TestFixture]
    class OptionalCommandsTests
    {
        private readonly GetterOfThingsAboutBotCommand _getterOfThings;
        private readonly TestController _controller;
        private readonly CommandParser _commandParser;
        private static readonly object[] _messagesAndExpectedTimesOrLists = new[]
        {
            new object[] { "-OptionalArgs -testtime 5d", TimeSpan.FromDays(5) },
            new object[] { "-custom_optional time 17m", TimeSpan.FromMinutes(17) },
            new object[] { "-OptionalArgs -testlist sth", new List<string> { "sth" } },
            new object[] { "-custom_optional list abc qwerty", new List<string> { "abc", "qwerty" } }
        };

        public OptionalCommandsTests()
        {
            this._getterOfThings = new GetterOfThingsAboutBotCommand();
            this._controller = new TestController();
            this._commandParser = new CommandParser();
        }

        [Test]
        [TestCase("-OptionalArgs -testnullableint 5", 5)]
        [TestCase("-custom_optional int -2", -2)]
        [TestCase("-OptionalArgs -testusermention <@12345>", 12345UL)]
        [TestCase("-custom_optional user <@!789>", 789UL)]
        [TestCase("-OptionalArgs -testchannelmention <#333>", 333UL)]
        [TestCase("-custom_optional channel <#877>", 877UL)]
        [TestCaseSource(nameof(_messagesAndExpectedTimesOrLists))]
        public async Task ShouldGiveExpectedValueForOptionalArg(string message, object expectedValue)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommands(new (Type type, string regexInText)[] 
            { 
                (typeof(OptionalArgsCommand), @"-custom_optional\s*int\s*(?<TestNullableInt>-?\d*)?"),
                (typeof(OptionalArgsCommand), @"-custom_optional\s*user\s*(?<TestUserMention>\<@!?\d+\>)?"),
                (typeof(OptionalArgsCommand), @"-custom_optional\s*channel\s*(?<TestChannelMention>\<#\d+\>)?"),
                (typeof(OptionalArgsCommand), @"-custom_optional\s*time\s*(?<TestTime>\d+(ms|d|h|m|s))?"),
                (typeof(OptionalArgsCommand), @"-custom_optional\s*list\s*(?<TestList>.*)?")
            });
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._controller);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._controller.GeneralValue, Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("-OptionalArgs")]
        [TestCase("-custom_optional")]
        public async Task ShouldGiveDefaultValuesWhenOptionalArgsIsGivenWithoutArgs(string message)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<OptionalArgsCommand>(@"-custom_optional");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._controller);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            var args = this._controller.OptionalArgs;
            Assert.That(args.TestNullableInt, Is.EqualTo(null));
            Assert.That(args.TestTime, Is.EqualTo(null));
            Assert.That(args.TestUserMention, Is.EqualTo(null));
            Assert.That(args.TestChannelMention, Is.EqualTo(null));
            Assert.That(args.TestStandardInt, Is.EqualTo(0));
            Assert.That(args.TestULong, Is.EqualTo(0UL));
            Assert.That(args.TestList, Is.EqualTo(null));
        }

        [Test]
        [TestCase("-OptionalArgs -testnullableint 5a")]
        [TestCase("-custom_optional int a")]
        [TestCase("-OptionalArgs -testusermention 12345")]
        [TestCase("-custom_optional user abc")]
        [TestCase("-OptionalArgs -testchannelmention <333>")]
        [TestCase("-custom_optional channel <#abc>")]
        [TestCase("-OptionalArgs -testtime 3")]
        [TestCase("-custom_optional time 14x")]
        [TestCase("-OptionalArgs -testlist sth\"")]
        [TestCase("-custom_optional list \"sth\" else\"")]
        public void ShouldThrowException_WhenOptionalArgIsGivenWithIncorrectValue(string message)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommands(new (Type type, string regexInText)[]
            {
                (typeof(OptionalArgsCommand), @"-custom_optional\s*int\s*(?<TestNullableInt>.*)?"),
                (typeof(OptionalArgsCommand), @"-custom_optional\s*user\s*(?<TestUserMention>.*)?"),
                (typeof(OptionalArgsCommand), @"-custom_optional\s*channel\s*(?<TestChannelMention>.*)?"),
                (typeof(OptionalArgsCommand), @"-custom_optional\s*time\s*(?<TestTime>.*)?"),
                (typeof(OptionalArgsCommand), @"-custom_optional\s*list\s*(?<TestList>.*)?")
            });
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._controller);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }

        [Test]
        [TestCase("-OptionalArgs -testnullableint")]
        [TestCase("-OptionalArgs -testulong      ")]
        [TestCase("-OptionalArgs    -testtime       ")]
        public void ShouldThrowException_WhenOptionalParametrIsGivenWithoutValue(string message)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._controller);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }
    }
}
