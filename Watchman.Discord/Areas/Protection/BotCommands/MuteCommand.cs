using Devscord.DiscordFramework.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Protection.BotCommands
{
    public class MuteCommand : IBotCommand
    {
        [List]
        public List<string> Users { get; set; }
        [List]
        public List<string> Times { get; set; }
        [List]
        public List<string> Reasons { get; set; }
    }
}
