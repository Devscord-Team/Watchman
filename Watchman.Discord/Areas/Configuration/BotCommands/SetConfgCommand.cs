using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Discord.Areas.Configuration.BotCommands
{
    public class SetConfgCommand : IBotCommand
    {
        [Text]
        public string Name { get; set; }

        [Text]
        public string Value { get; set; }
    }
}
