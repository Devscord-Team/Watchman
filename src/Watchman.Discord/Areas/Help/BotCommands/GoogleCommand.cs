using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Discord.Areas.Help.BotCommands
{
    public class GoogleCommand : IBotCommand
    {
        [Text]
        public string Search { get; set; }
    }
}
