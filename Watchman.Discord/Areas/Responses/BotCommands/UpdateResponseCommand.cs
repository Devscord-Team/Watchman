﻿using Devscord.DiscordFramework.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Responses.BotCommands
{
    public class UpdateResponseCommand : IBotCommand
    {
        [SingleWord]
        public string OnEvent { get; set; }
        [Text]
        public string Message { get; set; }
    }
}
