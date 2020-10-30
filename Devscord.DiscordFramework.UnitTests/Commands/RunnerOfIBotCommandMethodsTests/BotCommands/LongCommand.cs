using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands
{
    class LongCommand : IBotCommand
    {
        [Number]
        public long TestLong { get; set; }
    }
}
