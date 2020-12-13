using System;
using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands
{
    public class SomeCustomCommand : IBotCommand
    {
        [List]
        public List<string> List { get; set; }

        [Bool]
        public bool Bool { get; set; }

        [Number]
        [Optional]
        public int Number { get; set; }
    }
}
