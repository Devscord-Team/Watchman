using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Discord.Areas.Prefixes.Commands
{
    public class RemovePrefixCommand : IBotCommand
    {
        [Text]
        public string Prefix { get; set; }
    }
}
