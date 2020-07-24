using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

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
