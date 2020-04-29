using Devscord.DiscordFramework.Framework.Commands;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devscord.DiscordFramework.UnitTests.Commands
{
    [TestFixture]
    public class BotCommandsServiceTests
    {
        [Test]
        public void ShouldGenerateDefaultCommandTemplateBasedOnModel()
        {
            //Arrange
            var command = new TestCommand();
            var service = new BotCommandsService();

            //Act
            var template = service.GetCommandTemplate(command);

            //Assert
            Assert.That(template.CommandName, Is.EqualTo("TestCommand"));
            Assert.That(template.Properties.Count(), Is.EqualTo(5));
            Assert.That(template.Properties.Count(x => x.Type == BotCommandPropertyType.Text), Is.EqualTo(1));
            Assert.That(template.Properties.Count(x => x.Type == BotCommandPropertyType.SingleWord), Is.EqualTo(2));
            Assert.That(template.Properties.Count(x => x.Type == BotCommandPropertyType.UserMention), Is.EqualTo(1));
            Assert.That(template.Properties.Count(x => x.Type == BotCommandPropertyType.Time), Is.EqualTo(0));
        }
    }

    public class TestCommand : IBotCommand
    {
        [Text]
        public string TestText { get; set; }
        public string TestSingleWord { get; set; }
        public string TestWithoutAtribute { get; set; }
        [Number]
        public string TestNumber { get; set; }
        [UserMention]
        public string TestUser { get; set; }
    }
}
