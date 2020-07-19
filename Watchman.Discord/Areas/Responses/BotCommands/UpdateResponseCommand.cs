using Devscord.DiscordFramework.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Responses.BotCommands
{
    public class UpdateResponseCommand : IBotCommand
    {
        [List]
        public List<string> OnEvents { get; set; }
        [List]
        public List<string> Messages { get; set; }
    }
}
