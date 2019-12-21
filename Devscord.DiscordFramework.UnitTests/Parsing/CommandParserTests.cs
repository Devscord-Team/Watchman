using Devscord.DiscordFramework.Framework.Commands.Parsing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.UnitTests.Parsing
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
        [TestCase("help", "help", true)]
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
    }
}
