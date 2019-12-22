using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Services.Models;
using Watchman.DomainModel.Help.Models;

namespace Watchman.Discord.Areas.Help.Factories
{
    class ArgumentInfoFactory
    {
        public ArgumentInfo Create(CommandArgumentInfo argument)
        {
            return new ArgumentInfo
            {
                ArgumentPrefix = argument.ArgumentPrefix,
                Name = argument.Name
            };
        }
    }
}
