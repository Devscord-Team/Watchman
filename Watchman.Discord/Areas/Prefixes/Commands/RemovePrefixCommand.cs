using Devscord.DiscordFramework.Framework.Commands;
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
