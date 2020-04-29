using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Discord.Areas.UselessFeatures.BotCommands
{
    public class PrintCommand : IBotCommand
    {
        [Text]
        public string Message { get; set; }
        [Number]
        public int Times { get; set; }
        [Time]
        public TimeSpan Delay { get; set; }
    }
}
