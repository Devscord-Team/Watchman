using Devscord.DiscordFramework.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Protection.BotCommands
{
    public class UnmuteCommand : IBotCommand
    {
        [UserMention]
        public ulong User { get; set; }
    }
}
