using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands
{
    public class NumbersCommand : IBotCommand
    {
        [Number]
        public int TestInt { get; set; }

        [Number]
        public uint TestUInt { get; set; }

        [Number]
        public long TestLong { get; set; }

        [Number]
        public ulong TestULong { get; set; }

        [Number]
        public double TestDouble { get; set; }

        [Number]
        public decimal TestDecimal { get; set; }
    }
}
