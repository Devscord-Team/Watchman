using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Discord.Areas.Users.BotCommands.Warns
{
    public class ResetWarnsCommand : IBotCommand
    {
        [UserMention]
        public ulong User { get; set; }
    }
}
