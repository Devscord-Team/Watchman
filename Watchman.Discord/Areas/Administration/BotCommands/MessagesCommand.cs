using Devscord.DiscordFramework.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Administration.BotCommands
{
    public class MessagesCommand : IBotCommand
    {  
        [Text]
        public string Mention { get; set; }
        [Text]
        public string Time { get; set; }
        [Bool]
        public bool HasForceArgument { get; set; }
    }
}
