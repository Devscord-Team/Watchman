using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;

namespace Devscord.DiscordFramework.UnitTests.Commands
{
    public class TestCommand : IBotCommand
    {
        [Text]
        public string TestText { get; set; }
        public string TestSingleWord { get; set; }
        public string TestWithoutAtribute { get; set; }

        [Number]
        public int TestNumber { get; set; }

        [UserMention]
        public ulong TestUser { get; set; }
    }
}
