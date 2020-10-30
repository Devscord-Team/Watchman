using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands
{
    class OptionalArgsCommand : IBotCommand
    {
        [Optional]
        [Number]
        public int? TestNullableInt { get; set; }

        [Optional]
        [Time]
        public TimeSpan? TestTime { get; set; }

        [Optional]
        [UserMention]
        public ulong? TestUserMention { get; set; }

        [Optional]
        [ChannelMention]
        public ulong? TestChannelMention { get; set; }

        [Optional]
        [Number]
        public int TestStandardInt { get; set; }

        [Optional]
        [Number]
        public ulong TestULong { get; set; }

        [Optional]
        [List]
        public List<string> TestList { get; set; }
    }
}
