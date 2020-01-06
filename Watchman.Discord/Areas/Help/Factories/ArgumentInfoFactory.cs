using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Watchman.DomainModel.Help;

namespace Watchman.Discord.Areas.Help.Factories
{
    public class ArgumentInfoFactory : IService
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
