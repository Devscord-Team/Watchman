using Devscord.DiscordFramework.Services.Models;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Factories
{
    public class ArgumentInfoFactory
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
