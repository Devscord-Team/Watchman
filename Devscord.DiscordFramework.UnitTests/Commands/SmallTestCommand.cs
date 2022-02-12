using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;

namespace Devscord.DiscordFramework.UnitTests.Commands
{
    public class SmallTestCommand : IBotCommand
    {
        [Number]
        public int TestNumber { get; set; }

        [UserMention]
        [Optional]
        public ulong TestUser { get; set; }
    }
}
