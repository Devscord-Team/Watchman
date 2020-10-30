using System.Collections.Generic;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands
{
    class ListCommand : IBotCommand
    {
        [List]
        public List<string> TestList { get; set; }
    }
}
