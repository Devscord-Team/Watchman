using System.Collections.Generic;
using Devscord.DiscordFramework.Services.Models;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Factories
{
    public class ArgumentInfoFactory
    {
        public ArgumentInformation Create(BotArgumentInformation argument)
        {
            var defaultDescription = new List<Description>
            {
                new Description()
            };
            return new ArgumentInformation(argument.Name, argument.ExpectedType.Name, defaultDescription, argument.IsOptional);
        }
    }
}
