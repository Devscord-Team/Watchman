using Devscord.DiscordFramework.Services.Models;
using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Factories
{
    public class HelpInformationFactory
    {
        private readonly ArgumentInfoFactory _argumentInfoFactory;

        public HelpInformationFactory(ArgumentInfoFactory argumentInfoFactory)
        {
            this._argumentInfoFactory = argumentInfoFactory;
        }

        public HelpInformation Create(BotCommandInformation commandInformation)
        {
            return new HelpInformation
            {
                MethodFullName = commandInformation.MethodFullName,
                Names = commandInformation.Names,
                ArgumentInfos = commandInformation.BotCommandArgumentInformations.Select(x => this._argumentInfoFactory.Create(x)),
                ServerId = 0,
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
