using System;
using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands
{
    public class SomeDefaultCommand : IBotCommand
    {
        [Time]
        public TimeSpan Time { get; set; }

        [UserMention]
        public ulong User { get; set; }

        [List]
        public List<string> List { get; set; }
    }
}
