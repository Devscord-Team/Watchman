using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;

namespace Watchman.Discord.Areas.Statistics.BotCommands
{
    public class StatsCommand : IBotCommand
    {
        [Bool]
        public bool Minute { get; set; }
        [Bool]
        public bool Hour { get; set; }
        [Bool]
        public bool Day { get; set; }
        [Bool]
        public bool Week { get; set; }
        [Bool]
        public bool Month { get; set; }
        [Bool]
        public bool Quarter { get; set; }

        [Optional]
        [UserMention]
        public ulong User { get; set; }

        [Optional]
        [ChannelMention]
        public ulong Channel { get; set; }
    }
}
