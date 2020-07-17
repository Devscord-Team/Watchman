using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
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
        public string Reason { get; set; }
    }
}
