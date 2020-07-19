using Devscord.DiscordFramework.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Responses.BotCommands
{
    public class RemoveResponseCommand : IBotCommand
    {
        [List]
        public List<string> OnEvents { get; set; }
    }
}
