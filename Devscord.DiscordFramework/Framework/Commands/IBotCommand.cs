using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Framework.Commands
{
    public interface IBotCommand
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class UseCommand : Attribute
    {
        public string CommandName { get; private set; }

        public UseCommand(string commandName)
        {
            CommandName = commandName;
        }
    }
}
