using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Controllers;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using NUnit.Framework;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.Tests
{
    [TestFixture]
    class DefaultCommandsTests
    {
        private readonly GetterOfThingsAboutBotCommand _getterOfThings;
        private readonly TestController _testController;
        private readonly CommandParser _commandParser;
        private static readonly object[] _messagesAndExpectedTimes = new object[]
        {
            new object[] { "-time -testtime 1d", TimeSpan.FromDays(1) },
            new object[] { "-time -testtime 3h", TimeSpan.FromHours(3) },
            new object[] { "-time -testtime 12m", TimeSpan.FromMinutes(12) },
            new object[] { "-time -testtime 30s", TimeSpan.FromSeconds(30) },
            new object[] { "-time -testtime 500ms", TimeSpan.FromMilliseconds(500) },
        };

        public DefaultCommandsTests()
        {
            this._getterOfThings = new GetterOfThingsAboutBotCommand();
            this._testController = new TestController();
            this._commandParser = new CommandParser();
        }

        [Test]
        [TestCase("-int -testint 4", 4)]
        [TestCase("-int -testint   -2", -2)]
        [TestCase("-int     -testint -2121", -2121)]
        [TestCase("-int       -testint      45   ", 45)]
        public async Task ShouldGiveExpectedIntNumber(string message, int expectedInt)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestInt, Is.EqualTo(expectedInt));
        }

        [Test]
        [TestCase("-uint -testuint 5", 5U)]
        [TestCase("-uint     -testuint      345123   ", 345123U)]
        public async Task ShouldGiveUIntNumber(string message, uint expectedUInt)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestUInt, Is.EqualTo(expectedUInt));
        }

        [Test]
        [TestCase("-long -testlong 6", 6L)]
        [TestCase("-long -testlong       -99", -99L)]
        [TestCase("-long     -testlong      223456   ", 223456L)]
        public async Task ShouldGiveExpectedLongNumber(string message, long expectedLong)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestLong, Is.EqualTo(expectedLong));
        }

        [Test]
        [TestCase("-ulong -testulong 7", 7UL)]
        [TestCase("-ulong     -testulong      88891   ", 88891UL)]
        public async Task ShouldGiveULongNumber(string message, ulong expectedULong)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestULong, Is.EqualTo(expectedULong));
        }

        [Test]
        [TestCase("-double -testdouble 8", 8.0)]
        [TestCase("-double -testdouble       -103", -103.0)]
        [TestCase("-double     -testdouble      23,998   ", 23.998)]
        [TestCase("-double -testdouble -34,16     ", -34.16)]
        public async Task ShouldGiveExpectedDoubleNumber(string message, double expectedDouble)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestDouble, Is.EqualTo(expectedDouble));
        }

        [Test]
        [TestCase("-decimal -testdecimal 9", 9.0)]
        [TestCase("-decimal -testdecimal       -234", -234.0)]
        [TestCase("-decimal     -testdecimal      67,578   ", 67.578)]
        [TestCase("-decimal -testdecimal -12,18     ", -12.18)]
        public async Task ShouldGiveExpectedDecimalNumber(string message, decimal expectedDecimal)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestDecimal, Is.EqualTo(expectedDecimal));
        }

        [Test]
        [TestCase("-int -testint 6a", true)]
        [TestCase("-uint -testuint b", true)]
        [TestCase("-int -testint 67", false)]
        [TestCase("-long -testlong a6", true)]
        [TestCase("-ulong -testulong     c", true)]
        [TestCase("-double -testdouble d     ", true)]
        [TestCase("-decimal       -testdecimal   f1   ", true)]
        [TestCase("-uint -testuint   85 ", false)]
        [TestCase("-long    -testlong   56 ", false)]
        [TestCase("-double    -testdouble   67,89 ", false)]
        [TestCase("-double -testdouble 89.78", true)]
        public void ShouldThrowException_WhenNumberIsIncorrect(string message, bool shouldThrowException)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
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
        [TestCase("-text -testtext sth", "sth")]
        [TestCase("-text -testtext sth     ", "sth")]
        [TestCase("-text -testtext \"sth\"", "sth")]
        [TestCase("-text -testtext \"sth else\"", "sth else")]
        [TestCase("-text   -testtext     \"sth else\"    ", "sth else")]
        public async Task ShouldGiveText(string message, string expectedText)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestText, Is.EqualTo(expectedText));
        }

        [Test]
        [TestCase("-text -testtext sth\"", true)]
        [TestCase("-text -testtext sth", false)]
        [TestCase("-text -testtext \"sth\"", false)]
        // for a case of "sth it should be exception "InvalidArgumentsException" but it should be from CommandParser
        [TestCase("-list -testlist anything\"", true)]
        [TestCase("-list -testlist \"anything\"", false)]
        [TestCase("-list -testlist \"anything\" anything else\"", true)]
        public void ShouldThrowException_WhenItIsIncorrectNumberOfQuotationMarks(string message, bool shouldThrowException)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
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
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse("-bool -testbool", DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestBool);
        }

        [Test]
        public async Task ShouldGiveFalse_WhenBoolArgumentWasNotGiven()
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse("-bool", DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.False(this._testController.ValueOfTestBool);
        }

        [Test]
        [TestCase("-singleword -testsingleword first", "first")]
        [TestCase("-singleword    -testsingleword       one", "one")]
        [TestCase("-singleword -testsingleword 1st 2nd", "1st")]
        public async Task ShouldGiveOnlyFirstWord(string message, string expectedWord)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestSingleWord, Is.EqualTo(expectedWord));
        }

        [Test]
        [TestCase("-list -testlist arg1", "arg1")]
        [TestCase("-list -testlist arg1 arg2", "arg1", "arg2")]
        [TestCase("-list -testlist arg1 \"arg2\"", "arg1", "arg2")]
        [TestCase("-list -testlist arg1 \"an arg 2\"", "arg1", "an arg 2")]
        [TestCase("-list -testlist arg1 \"arg 2\" arg3", "arg1", "arg 2", "arg3")]
        [TestCase("-list -testlist arg1 \"arg 2\" arg3", "arg1", "arg 2", "arg3")]
        [TestCase("-list -testlist arg1 \"arg 2\" \"arg 3 3 3\"", "arg1", "arg 2", "arg 3 3 3")]
        [TestCase("-list -testlist \"arg 1\" arg2 \"arg 3 3 3\"", "arg 1", "arg2", "arg 3 3 3")]
        public async Task ShouldGiveList(string message, params string[] expectedArgs)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestList, Is.EqualTo(expectedArgs));
        }

        [Test]
        [TestCaseSource(nameof(_messagesAndExpectedTimes))]
        public async Task ShouldGiveExpectedTime(string message, TimeSpan expectedTime)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestTime, Is.EqualTo(expectedTime));
        }

        [Test]
        [TestCase("-time -testtime 40")]
        [TestCase("-time -testtime 4a")]
        [TestCase("-time -testtime fg")]
        [TestCase("-time -testtime ms")]
        public void ShouldThrowException_WhenGivenCommandHasIncorrectTime(string message)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }

        [Test]
        [TestCase("-usermention -testusermention <@123456789>", 123456789ul)]
        [TestCase("-usermention -testusermention <@!987654321>", 987654321ul)]
        public async Task ShouldGiveUserIdFromUserMention(string message, ulong expectedID)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestUserMention, Is.EqualTo(expectedID));
        }

        [Test]
        [TestCase("-usermention -testusermention <@>")]
        [TestCase("-usermention -testusermention <@!>")]
        [TestCase("-usermention -testusermention 12345")]
        [TestCase("-usermention -testusermention 3")]
        [TestCase("-usermention -testusermention abc")]
        public void ShouldThrowException_WhenGivenCommandHasIncorrectUserMention(string message)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
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
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse($"-channelmention -testchannelmention <#{expectedID}>", DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestChannelMention, Is.EqualTo(expectedID));
        }

        [Test]
        [TestCase("-channelmention -testchannelmention <#>")]
        [TestCase("-channelmention -testchannelmention 12345")]
        [TestCase("-channelmention -testchannelmention 3")]
        [TestCase("-channelmention -testchannelmention abc")]
        public void ShouldThrowException_WhenGivenCommandHasIncorrectChannelMention(string message)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            async Task RunMethodsFunc() => await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.ThrowsAsync<InvalidArgumentsException>(RunMethodsFunc);
        }

        [Test]
        [TestCase("-usermention -testusermention <@123>", 123UL)]
        [TestCase("-UserMention -TestUserMention <@333>", 333UL)]
        [TestCase("-USERMENTION -TESTUSERMENTION <@555>", 555UL)]
        public async Task ShouldProcessCommandCorrectlyRegardlessOfCase(string message, ulong expectedValue)
        {
            // Arrange
            var runner = this._getterOfThings.GetRunner();
            var contexts = this._getterOfThings.GetContexts();
            var controllerInfos = this._getterOfThings.GetListOfControllerInfo(this._testController);
            var request = this._commandParser.Parse(message, DateTime.Now);

            // Act
            await runner.RunMethodsIBotCommand(request, contexts, controllerInfos);

            // Assert
            Assert.That(this._testController.ValueOfTestUserMention, Is.EqualTo(expectedValue));
        }
    }
}
