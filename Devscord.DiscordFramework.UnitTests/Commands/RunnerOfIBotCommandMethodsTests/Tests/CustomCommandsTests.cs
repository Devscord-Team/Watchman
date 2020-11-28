using System;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Controllers;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using NUnit.Framework;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Tests
{
    [TestFixture]
    class CustomCommandsTests
    {
        private readonly GetterOfThingsAboutBotCommand _getterOfThings;
        private readonly TestController _testController;
        private readonly CommandParser _commandParser;
        private static readonly object[] _messagesAndExpectedTimes = new object[]
        {
            new object[] { "-custom time 1d", TimeSpan.FromDays(1) },
            new object[] { "-custom time 3h", TimeSpan.FromHours(3) },
            new object[] { "-custom time 12m", TimeSpan.FromMinutes(12) },
            new object[] { "-custom time 30s", TimeSpan.FromSeconds(30) },
            new object[] { "-custom time 500ms", TimeSpan.FromMilliseconds(500) },
        };

        public CustomCommandsTests()
        {
            this._getterOfThings = new GetterOfThingsAboutBotCommand();
            this._testController = new TestController();
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
            var customCommands = this._getterOfThings.CreateCustomCommandFor<IntCommand>(@"-custom\s*int\s*(?<TestInt>.*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestInt, Is.EqualTo(expectedInt));
        }

        [Test]
        [TestCase("-custom uint 5", 5U)]
        [TestCase("-custom     uint      345123   ", 345123U)]
        public async Task ShouldGiveUIntNumber(string message, uint expectedUInt)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<UIntCommand>(@"-custom\s*uint\s*(?<TestUInt>\d*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestUInt, Is.EqualTo(expectedUInt));
        }

        [Test]
        [TestCase("-custom long 6", 6L)]
        [TestCase("-custom long       -99", -99L)]
        [TestCase("-custom     long      223456   ", 223456L)]
        public async Task ShouldGiveExpectedLongNumber(string message, long expectedLong)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<LongCommand>(@"-custom\s*long\s*(?<TestLong>-?\d*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestLong, Is.EqualTo(expectedLong));
        }

        [Test]
        [TestCase("-custom ulong 7", 7UL)]
        [TestCase("-custom     ulong      88891   ", 88891UL)]
        public async Task ShouldGiveULongNumber(string message, ulong expectedULong)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<ULongCommand>(@"-custom\s*ulong\s*(?<TestULong>\d*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestULong, Is.EqualTo(expectedULong));
        }

        [Test]
        [TestCase("-custom double 8", 8.0)]
        [TestCase("-custom double       -103", -103.0)]
        [TestCase("-custom     double      23,998   ", 23.998)]
        [TestCase("-custom double -34,16     ", -34.16)]
        public async Task ShouldGiveExpectedDoubleNumber(string message, double expectedDouble)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<DoubleCommand>(@"-custom\s*double\s*(?<TestDouble>.*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestDouble, Is.EqualTo(expectedDouble));
        }

        [Test]
        [TestCase("-custom decimal 9", 9.0)]
        [TestCase("-custom decimal       -234", -234.0)]
        [TestCase("-custom     decimal      67,578   ", 67.578)]
        [TestCase("-custom decimal -12,18     ", -12.18)]
        public async Task ShouldGiveExpectedDecimalNumber(string message, decimal expectedDecimal)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<DecimalCommand>(@"-custom\s*decimal\s*(?<TestDecimal>.*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestDecimal, Is.EqualTo(expectedDecimal));
        }

        [Test]
        [TestCase("-custom int 6a", true)]
        [TestCase("-custom uint b", true)]
        [TestCase("-custom int 67", false)]
        [TestCase("-custom long a6", true)]
        [TestCase("-custom ulong     c", true)]
        [TestCase("-custom double d     ", true)]
        [TestCase("-custom uint   85 ", false)]
        [TestCase("-custom       decimal   f1   ", true)]
        [TestCase("-custom uint   85 ", false)]
        [TestCase("-custom    long   56 ", false)]
        [TestCase("-custom    double   67,89 ", false)]
        [TestCase("-custom double 89.78", true)]
        public void ShouldThrowException_WhenNumberIsIncorrect(string message, bool shouldThrowException)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommands(new (Type type, string regexInText)[]
            {
                (typeof(IntCommand), @"-custom\s*int\s*(?<TestInt>.*)"),
                (typeof(UIntCommand), @"-custom\s*uint\s*(?<TestUInt>.*)"),
                (typeof(LongCommand), @"-custom\s*long\s*(?<TestLong>.*)"),
                (typeof(ULongCommand), @"-custom\s*ulong\s*(?<TestULong>.*)"),
                (typeof(DoubleCommand), @"-custom\s*double\s*(?<TestDouble>.*)"),
                (typeof(DecimalCommand), @"-custom\s*decimal\s*(?<TestDecimal>.*)")
            });
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

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
            var customCommands = this._getterOfThings.CreateCustomCommandFor<TextCommand>(@"-custom\s*text\s*(?<TestText>.*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestText, Is.EqualTo(expectedText));
        }

        [Test]
        [TestCase("-custom text sth\"", true)]
        [TestCase("-custom text sth", false)]
        [TestCase("-custom text \"sth\"", false)]
        // for a case of "sth it should be exception "InvalidArgumentsException" but it should be from CommandParser
        [TestCase("-custom list anything\"", true)]
        [TestCase("-custom list \"anything\"", false)]
        [TestCase("-custom list \"anything\" anything else\"", true)]
        public void ShouldThrowException_WhenItIsIncorrectNumberOfQuotationMarks(string message, bool shouldThrowException)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommands(new (Type type, string regexInText)[]
            { 
                (typeof(TextCommand), @"-custom\s*text\s*(?<TestText>.*)"),
                (typeof(ListCommand), @"-custom\s*list\s*(?<TestList>.*)")
            });
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

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
            var customCommands = this._getterOfThings.CreateCustomCommandFor<BoolCommand>(@"-custom\s*bool\s*(?<TestBool>-arg)?");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse("-custom bool -arg", DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestBool);
        }

        [Test]
        public async Task ShouldGiveFalse_WhenBoolArgumentWasNotGiven()
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<BoolCommand>(@"-custom\s*bool\s*(?<TestBool>-arg)?");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse("-custom bool", DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.False(this._testController.ValueOfTestBool);
        }

        [Test]
        [TestCase("-custom singleword first", "first")]
        [TestCase("-custom singleword        one", "one")]
        [TestCase("-custom singleword 1st 2nd", "1st")]
        [TestCase("-custom singleword \"sth\"", "sth")]
        public async Task ShouldGiveOnlyFirstWord(string message, string expectedWord)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<SingleWordCommand>(@"-custom\s*singleword\s*(?<TestSingleWord>.*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestSingleWord, Is.EqualTo(expectedWord));
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
            var customCommands = this._getterOfThings.CreateCustomCommandFor<ListCommand>(@"-custom\s*list\s*(?<TestList>.*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestList.OrderBy(x => x), Is.EqualTo(expectedArgs.OrderBy(x => x)));
        }

        [Test]
        [TestCaseSource(nameof(_messagesAndExpectedTimes))]
        public async Task ShouldGiveExpectedTime(string message, TimeSpan expectedTime)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<TimeCommand>(@"-custom\s*time\s*(?<TestTime>\d+(ms|d|h|m|s))");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestTime, Is.EqualTo(expectedTime));
        }

        [Test]
        [TestCase("-custom time 40")]
        [TestCase("-custom time 4a")]
        [TestCase("-custom time fg")]
        [TestCase("-custom time ms")]
        public void ShouldThrowException_WhenGivenCommandHasIncorrectTime(string message)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<TimeCommand>(@"-custom\s*time\s*(?<TestTime>.*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }

        [Test]
        [TestCase("-custom usermention <@123456789>", 123456789ul)]
        [TestCase("-custom usermention <@!987654321>", 987654321ul)]
        public async Task ShouldGiveUserIdFromUserMention(string message, ulong expectedID)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<UserMentionCommand>(@"-custom\s*usermention\s*(?<TestUserMention>\<@!?\d+\>)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestUserMention, Is.EqualTo(expectedID));
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
            var customCommands = this._getterOfThings.CreateCustomCommandFor<UserMentionCommand>(@"-custom\s*usermention\s*(?<TestUserMention>.*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }

        [Test]
        public async Task ShouldGiveChannelIdFromChannelMention([Random(1000ul, 10001ul, 1)] ulong expectedID)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<ChannelMentionCommand>(@"-custom\s*channelmention\s*(?<TestChannelMention>\<#\d+\>)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse($"-custom channelmention <#{expectedID}>", DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestChannelMention, Is.EqualTo(expectedID));
        }

        [Test]
        [TestCase("-custom channelmention <#>")]
        [TestCase("-custom channelmention 12345")]
        [TestCase("-custom channelmention 3")]
        [TestCase("-custom channelmention abc")]
        public void ShouldThrowException_WhenGivenCommandHasIncorrectChannelMention(string message)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommandFor<ChannelMentionCommand>(@"-custom\s*channelmention\s*(?<TestChannelMention>.*)");
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }

        [Test]
        [TestCase("-customtime 500ms")]
        [TestCase("-custom list arg")]
        public void ShouldThrowMoreThanOneRegexHasBeenMatchedException_WhenGivenCommandMatchesMoreThanOneRegex(string message)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommands(new (Type type, string regexInText)[]
            { 
                (typeof(TimeCommand), @"-custom\s*time\s*(?<TestTime>\d+(ms|d|h|m|s))"),
                (typeof(TimeCommand), @"-customtime\s*(?<TestTime>.*)"),
                (typeof(ListCommand), @"-custom\s+list\s+(?<TestList>.+)"),
                (typeof(ListCommand), @"-custom\s*(list)?\s*(?<TestTime>arg)"),
            });
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.ThrowsAsync<MoreThanOneRegexHasBeenMatchedException>(RunMethodsFunc);
        }

        [Test]
        [TestCase("-custom b")]
        [TestCase("-custom")]
        [TestCase("-custom_bool -bool")]
        public void ShouldDoesNotThrowException_WhenGivenCommandMatchesOnlyOneRegex(string message)
        {
            // Arrange
            var customCommands = this._getterOfThings.CreateCustomCommands(new (Type type, string regexInText)[] 
            { 
                (typeof(BoolCommand), @"-custom\s+(?<TestBool>b)"),
                (typeof(BoolCommand), @"-custom_bool\s+(?<TestBool>-bool)")
            });
            var runner = this._getterOfThings.GetRunner(customCommands);
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.DoesNotThrowAsync(RunMethodsFunc);
        }
    }
}
