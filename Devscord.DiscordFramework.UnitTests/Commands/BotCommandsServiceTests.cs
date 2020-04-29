using Devscord.DiscordFramework.Framework.Commands;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.UnitTests.Commands
{
    [TestFixture]
    public class BotCommandsServiceTests
    {
        [Test]
        public void ShouldGenerateDefaultCommandTemplateBasedOnModel()
        {

        }
    }

    public class TestCommand : IBotCommand
    {
        [Text]
        public string TestText { get; set; }
        public 
    }
}
