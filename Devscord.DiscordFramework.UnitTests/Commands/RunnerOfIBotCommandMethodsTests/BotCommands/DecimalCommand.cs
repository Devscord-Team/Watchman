using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands
{
    public class DecimalCommand : IBotCommand
    {
        [Number]
        public decimal TestDecimal { get; set; }
    }
}
