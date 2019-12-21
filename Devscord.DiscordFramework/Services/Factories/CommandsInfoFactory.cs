using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Services.Factories
{
    class CommandsInfoFactory
    {
        public IEnumerable<CommandInfo> Create(Type controller)
        {
            controller.GetMethod().
        }
    }
}
