using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

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
        public string TestUser { get; set; }
    }
}
