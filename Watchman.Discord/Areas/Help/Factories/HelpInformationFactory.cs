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
                CommandName = commandInformation.Name,
                AreaName = commandInformation.AreaName,
                ArgumentInformations = commandInformation.BotCommandArgumentInformations.Select(x => this._argumentInfoFactory.Create(x)),
                ServerId = HelpInformation.EMPTY_SERVER_ID,
                Descriptions = new List<Description>
                {
                    new Description()
                }
            };
        }
    }
}
