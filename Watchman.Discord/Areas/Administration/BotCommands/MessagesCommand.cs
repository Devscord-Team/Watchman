using Devscord.DiscordFramework.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Administration.BotCommands
{
    public class MessagesCommand : IBotCommand
    {  
        [List]
        public List<string> Users { get; set; }
        [List]
        public List<string> Times { get; set; }
        [Bool]
        public bool Force { get; set; }
    }
}
