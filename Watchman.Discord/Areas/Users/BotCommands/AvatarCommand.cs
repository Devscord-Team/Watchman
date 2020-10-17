using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Discord.Areas.Users.BotCommands
{
    public class AvatarCommand : IBotCommand
    {
        [Optional]
        [UserMention]
        public ulong User { get; set; }
    }
}
