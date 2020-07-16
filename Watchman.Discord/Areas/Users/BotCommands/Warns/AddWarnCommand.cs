using Devscord.DiscordFramework.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Watchman.Discord.Areas.Users.BotCommands
{
    public class AddWarnCommand : IBotCommand
    {
        public string User { get; set; }
        public string Reason { get; set; }
    }
}
