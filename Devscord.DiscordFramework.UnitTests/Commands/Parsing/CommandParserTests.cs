﻿using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var result = commandParser.Parse(message, DateTime.UtcNow);

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
            var result = commandParser.Parse(message, DateTime.UtcNow);

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
            var parsed = commandParser.Parse(message, DateTime.UtcNow);
            var argument = parsed.Arguments.First();

            //Assert
            Assert.That(argument.Prefix, Is.EqualTo(argumentPrefix));
            Assert.That(argument.Name, Is.EqualTo(name));
            Assert.That(argument.Value, Is.EqualTo(value));
        }

        [Test]
        [TestCase("!help -format json -for admins", 2)]
        [TestCase("!help -format json !for admins", 2)]
        [TestCase("!help -format json !for admins -tested true", 3)]
        [TestCase("!help -format json", 1)]
        public void ShouldFoundManyArguments(string message, int argumentsAmount)
        {
            //Arrange
            var commandParser = new CommandParser();

            //Act
            var parsed = commandParser.Parse(message, DateTime.UtcNow);
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
            var result = commandParser.Parse(message, DateTime.UtcNow);

            //Assert
            Assert.That(result.OriginalMessage, Is.EqualTo(message));
        }

        [Test]
        [TestCase("-mute @test -time 1m -reason \"long val 1\"", "long val 1")]
        public void ShouldFindValue(string message, string expectedValue)
        {
            //Arrange
            var commandParser = new CommandParser();

            //Act
            var result = commandParser.Parse(message, DateTime.UtcNow);

            //Assert
            Assert.That(result.HasArgument(null, expectedValue));
        }

        [Test]
        [TestCase("w!help", "w!", true)]
        [TestCase("w!help", "-", false)]
        [TestCase("testhelp", "test", true)]
        [TestCase("aa bbtest", "aa bb", true)]
        public void ShouldSelectPrefixes(string message, string prefix, bool shouldAssert)
        {
            //Arrange
            var commandParser = new CommandParser();
            var prefixes = new Dictionary<ulong, string[]>() { { 0, new string[] { prefix } } };
            commandParser.SetServersPrefixes(prefixes);

            //Act
            var result = commandParser.Parse(message, DateTime.UtcNow);

            //Assert
            if (shouldAssert)
            {
                Assert.That(result.Prefix, Is.EqualTo(prefix));
            }
            else
            {
                Assert.That(result.Prefix, Is.Not.EqualTo(prefix));
            }
        }

        [TestCase("-mute <@12345678> -t 10s -r \"test")]
        public void ShouldThrowException(string message)
        {
            // Arrange
            var commandParser = new CommandParser();

            // Act
            void ParseFunc() => commandParser.Parse(message, DateTime.UtcNow);

            // Assert
            Assert.Throws<InvalidArgumentsException>(ParseFunc);

        }
    }
}
