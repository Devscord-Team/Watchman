using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Discord.Areas.Configurations.BotCommands
{
    public class ConfigurationsCommand : IBotCommand
    {
        [Optional]
        [SingleWord]
        public string Group { get; set; }
    }
}
