using System.Collections.Generic;
using Devscord.DiscordFramework.Services.Models;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Factories
{
    public interface IArgumentInfoFactory
    {
        ArgumentInformation Create(BotArgumentInformation argument);
    }

    public class ArgumentInfoFactory : IArgumentInfoFactory
    {
        public ArgumentInfoFactory()
        {
        }

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
