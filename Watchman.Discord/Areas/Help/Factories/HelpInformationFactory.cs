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
            var argumentInformations = commandInformation.BotCommandArgumentInformations.Select(x => this._argumentInfoFactory.Create(x));
            var desciptionCommand = typeof(Devscord.DiscordFramework.Framework.Commands.BotCommandDescription.CommandDescription).GetProperties()
                .Where(x => x.PropertyType.Name == "String" && x.Name == commandInformation.Name)
                .Select(prop =>
                { 
                    return prop.GetValue(prop)?.ToString();
                }).FirstOrDefault();
            var descriptions = string.IsNullOrEmpty(desciptionCommand) ? new List<Description>() : new List<Description> { new Description() { Language = "EN", Text = desciptionCommand } };
            return new HelpInformation(commandInformation.Name, commandInformation.AreaName, argumentInformations, descriptions, HelpInformation.EMPTY_SERVER_ID);
        }
    }
}
