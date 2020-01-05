using Devscord.DiscordFramework.Framework.Commands.Parsing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devscord.DiscordFramework.UnitTests.Commands.Parsing
{
    [TestFixture]
    public class CommandParserTests
    {
        [Test]
        [TestCase("-help", "-", true)]
        [TestCase("--help", "--", true)]
        [TestCase("--help", "-", false)]
        [TestCase("-help json", "-", true)]
        [TestCase("!help", "-", false)]
        [TestCase("!help", "!", true)]
        [TestCase("help", null, true)]
        [TestCase("help", "-", false)]
        public void ShouldFoundPrefix(string message, string prefix, bool shouldTrue)
        {
            //Arrange
            var commandParser = new CommandParser();

            //Act
            var result = commandParser.Parse(message);

            //Assert
            if (shouldTrue)
            {
                Assert.That(result.Prefix, Is.EqualTo(prefix));
            }
            else
            {
                Assert.That(result.Prefix, Is.Not.EqualTo(prefix));
            }
        }

        [Test]
        [TestCase("-help", "help", true)]
        [TestCase("help", "help", false)]
        [TestCase("!roles", "roles", true)]
        [TestCase("-good", "bad", false)]
        public void ShouldFoundName(string message, string name, bool shouldTrue)
        {
            //Arrange
            var commandParser = new CommandParser();

            //Act
            var result = commandParser.Parse(message);

            //Assert
            if (shouldTrue)
            {
                Assert.That(result.Name, Is.EqualTo(name));
            }
            else
            {
                Assert.That(result.Name, Is.Not.EqualTo(name));
            }
        }

        [Test]
        [TestCase("!help -format json", "-", "format", "json")]
        [TestCase("!help -format xml", "-", "format", "xml")]
        [TestCase("!help !format xml", "!", "format", "xml")]
        [TestCase("!help json", null, null, "json")]
        public void ShouldFoundArgument(string message, string argumentPrefix, string name, string value)
        {
            //Arrange
            var commandParser = new CommandParser();

            //Act
            var parsed = commandParser.Parse(message);
            var argument = parsed.Arguments.First();

            //Assert
            Assert.That(parsed.ArgumentsPrefix, Is.EqualTo(argument.Prefix));
            Assert.That(argument.Prefix, Is.EqualTo(argumentPrefix));
            Assert.That(argument.Name, Is.EqualTo(name));
            Assert.That(argument.Values.First(), Is.EqualTo(value));
        }

        [Test]
        [TestCase("!help -format json -for admins", 2)]
        [TestCase("!help -format json !for admins", 1)]
        [TestCase("!help -format json !for admins -tested true", 2)]
        [TestCase("!help -format json", 1)]
        public void ShouldFoundManyArguments(string message, int argumentsAmount)
        {
            //Arrange
            var commandParser = new CommandParser();

            //Act
            var parsed = commandParser.Parse(message);
            var result = parsed.Arguments.Count();

            //Assert
            Assert.That(result, Is.EqualTo(argumentsAmount));
        }

        [Test]
        [TestCase("normal message")]
        [TestCase("not command!")]
        [TestCase("???")]
        [TestCase("-for example")]
        public void ShouldWorksWithNormalMessages(string message)
        {
            //Arrange
            var commandParser = new CommandParser();

            //Act
            var result = commandParser.Parse(message);

            //Assert
            Assert.That(result.OriginalMessage, Is.EqualTo(message));
        }
    }
}
