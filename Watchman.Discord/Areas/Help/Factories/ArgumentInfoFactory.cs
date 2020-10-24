using Devscord.DiscordFramework.Services.Models;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Factories
{
    public class ArgumentInfoFactory
    {
        public ArgumentInfo Create(BotArgumentInformation argument)
        {
            return new ArgumentInfo
            {
                Name = argument.Name,
                Description = "Empty",
                ExampleValues = "Empty"
            };
        }
    }
}
