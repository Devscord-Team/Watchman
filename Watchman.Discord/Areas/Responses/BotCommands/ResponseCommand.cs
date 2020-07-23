using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;

namespace Watchman.Discord.Areas.Responses.BotCommands
{
    public class ResponsesCommand : IBotCommand
    {
        [Text]
        public string Type { get; set; }
    }
}
