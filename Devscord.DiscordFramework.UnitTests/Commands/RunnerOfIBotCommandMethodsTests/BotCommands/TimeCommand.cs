using System;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands
{
    class TimeCommand : IBotCommand
    {
        [Time]
        public TimeSpan TestTime { get; set; }
    }
}
