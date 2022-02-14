using Devscord.DiscordFramework.Services.Models;
using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Factories
{
    public interface IHelpInformationFactory
    {
        HelpInformation Create(BotCommandInformation commandInformation);
    }

    public class HelpInformationFactory : IHelpInformationFactory
    {
        private readonly IArgumentInfoFactory _argumentInfoFactory;

        public HelpInformationFactory(IArgumentInfoFactory argumentInfoFactory)
        {
            this._argumentInfoFactory = argumentInfoFactory;
        }

        public HelpInformation Create(BotCommandInformation commandInformation)
        {
            var argumentInformations = commandInformation.BotCommandArgumentInformations.Select(x => this._argumentInfoFactory.Create(x));
            var descriptions = new List<Description> { new Description() };
            return new HelpInformation(commandInformation.Name, commandInformation.AreaName, argumentInformations, descriptions, HelpInformation.EMPTY_SERVER_ID);
        }
    }
}
