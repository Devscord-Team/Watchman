using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Messaging.BotCommands
{
    public class SendCommand : IBotCommand
    {
        [ChannelMention]
        public ulong Channel { get; set; }

        [Text]
        public string Message { get; set; }
    }
}
