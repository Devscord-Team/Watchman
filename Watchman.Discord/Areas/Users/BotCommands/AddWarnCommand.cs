using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Watchman.Discord.Areas.Users.BotCommands
{
    public class AddWarnCommand : IBotCommand
    {
        [UserMention]
        public ulong User { get; set; }
        [Text]
        public string Reason { get; set; }
    }
}
