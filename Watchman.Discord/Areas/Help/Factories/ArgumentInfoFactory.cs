using System.Collections.Generic;
using Devscord.DiscordFramework.Services.Models;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Factories
{
    public class ArgumentInfoFactory
    {
        public ArgumentInformation Create(BotArgumentInformation argument)
        {
            return new ArgumentInformation
            {
                Name = argument.Name,
                IsOptional = argument.IsOptional,
                ExpectedTypeName = argument.ExpectedType.Name,
                Descriptions = new List<Description>
                {
                    new Description
                    {
                        Language = "EN",
                        Text = "Empty"
                    }
                }
            };
        }
    }
}
