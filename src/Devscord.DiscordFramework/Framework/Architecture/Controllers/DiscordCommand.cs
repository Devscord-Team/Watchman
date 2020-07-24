﻿using System;

namespace Devscord.DiscordFramework.Framework.Architecture.Controllers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DiscordCommand : Attribute
    {
        public string Command { get; private set; }
        public DiscordCommand(string command)
        {
            this.Command = command;
        }
    }
}
