using Devscord.DiscordFramework.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Administration.BotCommands
{
    public class MessagesCommand : IBotCommand
    {  
        [UserMention]
        public ulong User { get; set; }
        [Time]
        public TimeSpan Time { get; set; }
        [Bool]
        public bool Force { get; set; }
    }
}
