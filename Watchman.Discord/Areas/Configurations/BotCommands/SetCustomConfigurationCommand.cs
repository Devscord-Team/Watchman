using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;

namespace Watchman.Discord.Areas.Configurations.BotCommands
{
    // TODO: add option for list of each type not only string
    public class SetCustomConfigurationCommand : IBotCommand
    {
        [SingleWord]
        public string Name { get; set; }

        [Optional]
        [Text]
        public string TextValue { get; set; }

        [Optional]
        [Number]
        public double? NumberValue { get; set; }

        [Optional]
        [Number]
        public decimal? DecimalValue { get; set; }

        [Optional]
        [Time]
        public DateTime? TimeValue { get; set; }

        [Optional]
        [UserMention]
        public ulong? UserValue { get; set; }

        [Optional]
        [ChannelMention]
        public ulong? ChannelValue { get; set; }

        [Optional]
        [Text]
        public string BoolValue { get; set; }

        [Optional]
        [List]
        public List<string> ListValue { get; set; }
    }
}
