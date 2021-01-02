using System;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Controllers;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Tests
{
    [TestFixture]
    class CustomCommandsTests
    {
        private static readonly object[] _messagesAndExpectedTimes = new[]
        {
            new object[] { "-custom time 1d", TimeSpan.FromDays(1) },
            new object[] { "-custom time 3h", TimeSpan.FromHours(3) },
            new object[] { "-custom time 12m", TimeSpan.FromMinutes(12) },
            new object[] { "-custom time 30s", TimeSpan.FromSeconds(30) },
            new object[] { "-custom time 500ms", TimeSpan.FromMilliseconds(500) },
        };
        private static readonly object[] _customCommandsAndExpectedValues = new[]
        {
            new object[] { "-custom_optional time 17m", @"-custom_optional\s*time\s*(?<TestTime>\d+(ms|d|h|m|s))?", TimeSpan.FromMinutes(17) },
            new object[] { "-custom_optional list abc qwerty", @"-custom_optional\s*list\s*(?<TestList>.*)?", new List<string> { "abc", "qwerty" } }
        };
        private readonly CommandParser _commandParser;

        public CustomCommandsTests()
        {
            this._commandParser = new CommandParser();
        }

        [Test]
        [TestCase("-custom int 4", 4)]
        [TestCase("-custom int   -2", -2)]
        [TestCase("-custom     int -2121", -2121)]
        [TestCase("-custom       int      45   ", 45)]
        public async Task ShouldGiveExpectedIntNumber(string message, int expectedInt)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*int\s*(?<TestInt>.*)", typeof(IntCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            Assert.That(controller.ValueOfTestInt, Is.EqualTo(expectedInt));
        }

        [Test]
        [TestCase("-custom uint 5", 5U)]
        [TestCase("-custom     uint      345123   ", 345123U)]
        public async Task ShouldGiveUIntNumber(string message, uint expectedUInt)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*uint\s*(?<TestUInt>\d*)", typeof(UIntCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert  
            Assert.That(controller.ValueOfTestUInt, Is.EqualTo(expectedUInt));
        }

        [Test]
        [TestCase("-custom long 6", 6L)]
        [TestCase("-custom long       -99", -99L)]
        [TestCase("-custom     long      223456   ", 223456L)]
        public async Task ShouldGiveExpectedLongNumber(string message, long expectedLong)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*long\s*(?<TestLong>-?\d*)", typeof(LongCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert   
            Assert.That(controller.ValueOfTestLong, Is.EqualTo(expectedLong));
        }

        [Test]
        [TestCase("-custom ulong 7", 7UL)]
        [TestCase("-custom     ulong      88891   ", 88891UL)]
        public async Task ShouldGiveULongNumber(string message, ulong expectedULong)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*ulong\s*(?<TestULong>\d*)", typeof(ULongCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert    
            Assert.That(controller.ValueOfTestULong, Is.EqualTo(expectedULong));
        }

        [Test]
        [SetCulture("pl-PL")]   // for Github tests. The tests run right only for polish culture 
        [TestCase("-custom double 8", 8.0)]
        [TestCase("-custom double       -103", -103.0)]
        [TestCase("-custom     double      23,998   ", 23.998)]
        [TestCase("-custom double -34,16     ", -34.16)]
        public async Task ShouldGiveExpectedDoubleNumber(string message, double expectedDouble)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*double\s*(?<TestDouble>.*)", typeof(DoubleCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert 
            Assert.That(controller.ValueOfTestDouble, Is.EqualTo(expectedDouble));
        }

        [Test]
        [SetCulture("pl-PL")]
        [TestCase("-custom decimal 9", 9.0)]
        [TestCase("-custom decimal       -234", -234.0)]
        [TestCase("-custom     decimal      67,578   ", 67.578)]
        [TestCase("-custom decimal -12,18     ", -12.18)]
        public async Task ShouldGiveExpectedDecimalNumber(string message, decimal expectedDecimal)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*decimal\s*(?<TestDecimal>.*)", typeof(DecimalCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            Assert.That(controller.ValueOfTestDecimal, Is.EqualTo(expectedDecimal));
        }

        [Test]
        [SetCulture("pl-PL")]
        [TestCase("-custom int 6a", @"-custom\s*int\s*(?<TestInt>.*)", typeof(IntCommand), true)]
        [TestCase("-custom uint b", @"-custom\s*uint\s*(?<TestUInt>.*)", typeof(UIntCommand), true)]
        [TestCase("-custom int 67", @"-custom\s*int\s*(?<TestInt>.*)", typeof(IntCommand), false)]
        [TestCase("-custom long a6", @"-custom\s*long\s*(?<TestLong>.*)", typeof(LongCommand), true)]
        [TestCase("-custom ulong     c", @"-custom\s*ulong\s*(?<TestULong>.*)", typeof(ULongCommand), true)]
        [TestCase("-custom double d     ", @"-custom\s*double\s*(?<TestDouble>.*)", typeof(DoubleCommand), true)]
        [TestCase("-custom uint   85 ", @"-custom\s*uint\s*(?<TestUInt>.*)", typeof(UIntCommand), false)]
        [TestCase("-custom       decimal   f1   ", @"-custom\s*decimal\s*(?<TestDecimal>.*)", typeof(DecimalCommand), true)]
        [TestCase("-custom uint   85 ", @"-custom\s*uint\s*(?<TestUInt>.*)", typeof(UIntCommand), false)]
        [TestCase("-custom    long   56 ", @"-custom\s*long\s*(?<TestLong>.*)", typeof(LongCommand), false)]
        [TestCase("-custom    double   67,89 ", @"-custom\s*double\s*(?<TestDouble>.*)", typeof(DoubleCommand), false)]
        [TestCase("-custom double 89.78", @"-custom\s*double\s*(?<TestDouble>.*)", typeof(DoubleCommand), true)]
        public void ShouldThrowException_WhenNumberIsIncorrect(string message, string regex, Type type, bool shouldThrowException)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(regex, type);
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            if (shouldThrowException)
            {
                Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
            }
            else
            {
                Assert.DoesNotThrowAsync(RunMethodsFunc);
            }
        }

        [Test]
        [TestCase("-custom text sth", "sth")]
        [TestCase("-custom text sth     ", "sth")]
        [TestCase("-custom text \"sth\"", "sth")]
        [TestCase("-custom text \"sth else\"", "sth else")]
        [TestCase("-custom text     \"sth else\"    ", "sth else")]
        public async Task ShouldGiveText(string message, string expectedText)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*text\s*(?<TestText>.*)", typeof(TextCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            Assert.That(controller.ValueOfTestText, Is.EqualTo(expectedText));
        }

        [Test]
        [TestCase("-custom text sth\"", @"-custom\s*text\s*(?<TestText>.*)", typeof(TextCommand), true)]
        [TestCase("-custom text sth", @"-custom\s*text\s*(?<TestText>.*)", typeof(TextCommand), false)]
        [TestCase("-custom text \"sth\"", @"-custom\s*text\s*(?<TestText>.*)", typeof(TextCommand), false)]
        // for a case of "sth it should be exception "InvalidArgumentsException" but it should be from CommandParser
        [TestCase("-custom list anything\"", @"-custom\s*list\s*(?<TestList>.*)", typeof(ListCommand), true)]
        [TestCase("-custom list \"anything\"", @"-custom\s*list\s*(?<TestList>.*)", typeof(ListCommand), false)]
        [TestCase("-custom list \"anything\" anything else\"", @"-custom\s*list\s*(?<TestList>.*)", typeof(ListCommand), true)]
        public void ShouldThrowException_WhenItIsIncorrectNumberOfQuotationMarks(string message, string regex, Type type, bool shouldThrowException)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(regex, type);
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            if (shouldThrowException)
            {
                Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
            }
            else
            {
                Assert.DoesNotThrowAsync(RunMethodsFunc);
            }
        }

        [Test]
        public async Task ShouldGiveTrue_WhenBoolArgumentWasGiven()
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*bool\s*(?<TestBool>-arg)?", typeof(BoolCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse("-custom bool -arg", DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert 
            Assert.That(controller.ValueOfTestBool);
        }

        [Test]
        public async Task ShouldGiveFalse_WhenBoolArgumentWasNotGiven()
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*bool\s*(?<TestBool>-arg)?", typeof(BoolCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse("-custom bool", DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert  
            Assert.False(controller.ValueOfTestBool);
        }

        [Test]
        [TestCase("-custom singleword first", "first")]
        [TestCase("-custom singleword        one", "one")]
        [TestCase("-custom singleword 1st 2nd", "1st")]
        [TestCase("-custom singleword \"sth\"", "sth")]
        public async Task ShouldGiveOnlyFirstWord(string message, string expectedWord)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*singleword\s*(?<TestSingleWord>.*)", typeof(SingleWordCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert  
            Assert.That(controller.ValueOfTestSingleWord, Is.EqualTo(expectedWord));
        }

        [Test]
        [TestCase("-custom list arg1", "arg1")]
        [TestCase("-custom list arg1 arg2", "arg1", "arg2")]
        [TestCase("-custom list arg1 \"arg2\"", "arg1", "arg2")]
        [TestCase("-custom list arg1 \"an arg 2\"", "arg1", "an arg 2")]
        [TestCase("-custom list arg1 \"arg 2\" arg3", "arg1", "arg 2", "arg3")]
        [TestCase("-custom list arg1 \"arg 2\" arg3", "arg1", "arg 2", "arg3")]
        [TestCase("-custom list arg1 \"arg 2\" \"arg 3 3 3\"", "arg1", "arg 2", "arg 3 3 3")]
        [TestCase("-custom list \"arg 1\" arg2 \"arg 3 3 3\"", "arg 1", "arg2", "arg 3 3 3")]
        public async Task ShouldGiveList(string message, params string[] expectedArgs)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*list\s*(?<TestList>.*)", typeof(ListCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert   
            Assert.That(controller.ValueOfTestList.OrderBy(x => x), Is.EqualTo(expectedArgs.OrderBy(x => x)));
        }

        [Test]
        [TestCaseSource(nameof(_messagesAndExpectedTimes))]
        public async Task ShouldGiveExpectedTime(string message, TimeSpan expectedTime)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*time\s*(?<TestTime>\d+(ms|d|h|m|s))", typeof(TimeCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert 
            Assert.That(controller.ValueOfTestTime, Is.EqualTo(expectedTime));
        }

        [Test]
        [TestCase("-custom time 40")]
        [TestCase("-custom time 4a")]
        [TestCase("-custom time fg")]
        [TestCase("-custom time ms")]
        public void ShouldThrowException_WhenGivenCommandHasIncorrectTime(string message)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*time\s*(?<TestTime>.*)", typeof(TimeCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }

        [Test]
        [TestCase("-custom usermention <@123456789>", 123456789ul)]
        [TestCase("-custom usermention <@!987654321>", 987654321ul)]
        public async Task ShouldGiveUserIdFromUserMention(string message, ulong expectedID)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*usermention\s*(?<TestUserMention>\<@!?\d+\>)", typeof(UserMentionCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert  
            Assert.That(controller.ValueOfTestUserMention, Is.EqualTo(expectedID));
        }

        [Test]
        [TestCase("-custom usermention <@>")]
        [TestCase("-custom usermention <@!>")]
        [TestCase("-custom usermention 12345")]
        [TestCase("-custom usermention 3")]
        [TestCase("-custom usermention abc")]
        public void ShouldThrowException_WhenGivenCommandHasIncorrectUserMention(string message)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*usermention\s*(?<TestUserMention>.*)", typeof(UserMentionCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert 
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }

        [Test]
        public async Task ShouldGiveChannelIdFromChannelMention([Random(1000ul, 10001ul, 1)] ulong expectedID)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*channelmention\s*(?<TestChannelMention>\<#\d+\>)", typeof(ChannelMentionCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse($"-custom channelmention <#{expectedID}>", DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert 
            Assert.That(controller.ValueOfTestChannelMention, Is.EqualTo(expectedID));
        }

        [Test]
        [TestCase("-custom channelmention <#>")]
        [TestCase("-custom channelmention 12345")]
        [TestCase("-custom channelmention 3")]
        [TestCase("-custom channelmention abc")]
        public void ShouldThrowException_WhenGivenCommandHasIncorrectChannelMention(string message)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom\s*channelmention\s*(?<TestChannelMention>.*)", typeof(ChannelMentionCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert 
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }

        [Test]
        [TestCase("-custom_text    ", typeof(TextCommand), @"-custom_text\s*(?<TestText>.*)")]
        [TestCase("-custom_user", typeof(UserMentionCommand), @"-custom_user\s*(?<TestUserMention>.*)")]
        [TestCase("-somedefault -t  -l sth -u <@123>", typeof(SomeDefaultCommand), @"-somedefault\s*-t\s*(?<Time>.*)\s*-l\s*(?<List>.+)\s*-u\s*(?<User>.+)")]
        public void ShouldThrowException_WhenCommandIsGivenWithoutAllRequiredArgs(string message, Type type, string regex)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(regex, type);
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }

        [Test]
        [TestCase("-custom_text anything", typeof(TextCommand), @"-custom_text\s*(?<TestText>.*)")]
        [TestCase("-custom_user <@123>", typeof(UserMentionCommand), @"-custom_user\s*(?<TestUserMention>.*)")]
        [TestCase("-somedefault -t 4h -l sth else -u <@123>", typeof(SomeDefaultCommand), @"-somedefault\s*-t\s*(?<Time>\d+(m|ms|d|s|h))\s*-l\s*(?<List>.+)\s*-u\s*(?<User>.+)")]
        public void ShouldDoesNotThrowException_WhenGivenCommandHasEveryRequiredArg(string message, Type type, string regex)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(regex, type);
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            Assert.DoesNotThrowAsync(RunMethodsFunc);
        }

        [Test]
        [TestCase("-custom_optional int -2", @"-custom_optional\s*int\s*(?<TestNullableInt>-?\d*)?", -2)]
        [TestCase("-custom_optional user <@!789>", @"-custom_optional\s*user\s*(?<TestUserMention>\<@!?\d+\>)?", 789UL)]
        [TestCase("-custom_optional channel <#877>", @"-custom_optional\s*channel\s*(?<TestChannelMention>\<#\d+\>)?", 877UL)]
        [TestCaseSource(nameof(_customCommandsAndExpectedValues))]
        public async Task ShouldGiveExpectedValueForOptionalArg(string message, string regex, object expectedValue)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(regex, typeof(OptionalArgsCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            Assert.That(controller.GeneralValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public async Task ShouldGiveDefaultValuesWhenOptionalArgsIsGivenWithoutArgs()
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-custom_optional", typeof(OptionalArgsCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse("-custom_optional", DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            var args = controller.OptionalArgs;
            Assert.That(args.TestNullableInt, Is.EqualTo(null));
            Assert.That(args.TestTime, Is.EqualTo(null));
            Assert.That(args.TestUserMention, Is.EqualTo(null));
            Assert.That(args.TestChannelMention, Is.EqualTo(null));
            Assert.That(args.TestStandardInt, Is.EqualTo(0));
            Assert.That(args.TestULong, Is.EqualTo(0UL));
            Assert.That(args.TestList, Is.EqualTo(null));
        }

        [Test]
        [TestCase("-custom_optional int a", @"-custom_optional\s*int\s*(?<TestNullableInt>.*)?")]
        [TestCase("-custom_optional user abc", @"-custom_optional\s*user\s*(?<TestUserMention>.*)?")]
        [TestCase("-custom_optional channel <#abc>", @"-custom_optional\s*channel\s*(?<TestChannelMention>.*)?")]
        [TestCase("-custom_optional time 14x", @"-custom_optional\s*time\s*(?<TestTime>.*)?")]
        [TestCase("-custom_optional list \"sth\" else\"", @"-custom_optional\s*list\s*(?<TestList>.*)?")]
        public void ShouldThrowException_WhenOptionalArgIsGivenWithIncorrectValue(string message, string regex)
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(regex, typeof(OptionalArgsCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse(message, DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }

        [Test]
        public async Task ShouldGiveCorrectArgs_WhenGivenCommandIsActuallyDefaultCommandButMatchesCustomVersionOfCommand()
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-somedefault\s*(?<Time>.*)\s*(?<User>.*)\s*(?<List>.*)", typeof(SomeDefaultCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse("-somedefault -time 4m -user <@123> -list abc qwerty", DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            var args = controller.DefaultCommandArgs;
            Assert.That(args.Time, Is.EqualTo(TimeSpan.FromMinutes(4)));
            Assert.That(args.User, Is.EqualTo(123UL));
            Assert.That(args.List, Is.EqualTo(new List<string> { "abc", "qwerty" }));
        }

        [Test]
        public async Task ShouldGiveRightArgs_WhenCommandIsCustomCommandButMatchesDefaultVersionOfCommand()
        {
            // Arrange
            var botCommandsService = RunnerOfIBotCommandMethodsTestsService.GetBotCommandsServiceMock();
            var commandsContainer = RunnerOfIBotCommandMethodsTestsService.GetCommandsContainerMock(@"-somecustom\s*-list\s*(?<List>.*)\s*(?<Bool>-b|-bool)\s*(?<Number>.*)", typeof(SomeCustomCommand));
            var validator = RunnerOfIBotCommandMethodsTestsService.GetCommandMethodValidatorMock();
            var runner = new RunnerOfIBotCommandMethods(botCommandsService, commandsContainer, validator);
            var request = this._commandParser.Parse("-somecustom -list 123 456 789 -b 123", DateTime.Now);
            var controller = new Mock<TestController>().Object;

            // Act
            await runner.RunMethodsIBotCommand(request, contexts: null, new List<ControllerInfo> { new ControllerInfo(controller) });

            // Assert
            var args = controller.CustomCommandArgs;
            Assert.That(args.List, Is.EqualTo(new List<string> { "123", "456", "789" }));
            Assert.That(args.Bool, Is.EqualTo(true));
            Assert.That(args.Number, Is.EqualTo(123));
        }
    }
}
