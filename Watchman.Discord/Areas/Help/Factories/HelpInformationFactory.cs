using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devscord.DiscordFramework.Services.Models;
using Watchman.DomainModel.Help.Models;

namespace Watchman.Discord.Areas.Help.Factories
{
    class HelpInformationFactory
    {
        public HelpInformation Create(CommandInfo commandInfo)
        {
            var argumentFactory = new ArgumentInfoFactory();
            return new HelpInformation
            {
                Prefix = commandInfo.Prefix,
                MethodName = commandInfo.MethodName,
                Names = commandInfo.Names,
                ArgumentInfos = commandInfo.CommandArgumentInfos.Select(x => argumentFactory.Create(x)),
                ServerId = 0,
                DefaultDescriptionName = "EN",
                Descriptions = new List<Description>
                {
                    new Description
                    {
                        Name = "EN",
                        Details = "Empty"
                    }
                }
            };
        }
    }
}
