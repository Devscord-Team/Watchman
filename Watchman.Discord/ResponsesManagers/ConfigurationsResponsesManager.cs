using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;

namespace Watchman.Discord.ResponsesManagers
{
    public static class ConfigurationsResponsesManager
    {
        public static string ConfigurationItemNotFound(this IResponsesService responsesService, string name)
        {
            return responsesService.ProcessResponse("ConfigurationItemNotFound",
                new KeyValuePair<string, string>(nameof(name), name));
        }

        public static string TooManyValueArgumentsForSetConfiguration(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse("TooManyValueArgumentsForSetConfiguration");
        }

        public static string CustomConfigurationHasBeenSet(this IResponsesService responsesService, Contexts contexts, string name)
        {
            return responsesService.ProcessResponse("CustomConfigurationHasBeenSet", contexts,
                new KeyValuePair<string, string>(nameof(name), name));
        }

        public static string ConfigurationValueHasBeenSetAsDefaultOfType(this IResponsesService responsesService, Contexts contexts, string name)
        {
            return responsesService.ProcessResponse("ConfigurationValueHasBeenSetAsDefaultOfType", contexts,
                new KeyValuePair<string, string>(nameof(name), name));
        }
    }
}
