using Devscord.DiscordFramework.Commands.Parsing;
using NUnit.Framework;
using System;

namespace Devscord.DiscordFramework.UnitTests.Commands.Parsing.Models
{
    [TestFixture]
    public class DiscordRequestTests
    {
        [Test]
        [TestCase("-help -json", "json", true)]
        [TestCase("-help", "json", false)]
        [TestCase("-add", "role", false)]
        [TestCase("-add role", "role", false)]
        [TestCase("-add -role", "role", true)]
        [TestCase("-mute @test -time 1h", "time", true)]
        public void ShouldFoundName(string message, string name, bool shouldTrue)
        {
            //Arrange
            var commandParser = new CommandParser();

            //Act
            var result = commandParser.Parse(message, DateTime.UtcNow);

            //Assert
            Assert.AreEqual(shouldTrue, result.HasArgument(name));
        }

        [Test]
        [TestCase("-help json", "", "json", true)]
        [TestCase("-help -json xml", "json", "xml", true)]
        [TestCase("-help -json json", "json", "json", true)]
        [TestCase("-add tester", null, "tester", true)]
        [TestCase("-add tester", null, "csharp", false)]
        [TestCase("-help xml format", "format", "xml", false)]
        [TestCase("-help format xml", "format", "json", false)]
        [TestCase("-mute val1 val2 val3", null, "val2", true)]
        [TestCase("-mute val1 val2 val3", null, "val3", true)]
        [TestCase("-mute val1 val2 val3", null, "val4", false)]
        [TestCase("-mute val1 val2 val3", null, "val1", true)]
        public void ShouldFoundNameAndValue(string message, string name, string value, bool shouldTrue)
        {
            //Arrange
            var commandParser = new CommandParser();

            //Act
            var result = commandParser.Parse(message, DateTime.UtcNow);

            //Assert
            Assert.AreEqual(shouldTrue, result.HasArgument(name, value));
        }
    }
}
