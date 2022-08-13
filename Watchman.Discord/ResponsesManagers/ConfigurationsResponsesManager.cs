using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;

namespace Watchman.Discord.ResponsesManagers
{
    public static class ConfigurationsResponsesManager
    {
        public static string ConfigurationItemNotFound(this IResponsesService responsesService, string name)
        {
            return responsesService.ProcessResponse(nameof(ConfigurationItemNotFound),
                new KeyValuePair<string, string>(nameof(name), name));
        }

        public static string TooManyValueArgumentsForSetConfiguration(this IResponsesService responsesService)
        {
            return responsesService.ProcessResponse(nameof(TooManyValueArgumentsForSetConfiguration));
        }

        public static string CustomConfigurationHasBeenSet(this IResponsesService responsesService, Contexts contexts, string name)
        {
            return responsesService.ProcessResponse(nameof(CustomConfigurationHasBeenSet), contexts,
                new KeyValuePair<string, string>(nameof(name), name));
        }

        public static string ConfigurationValueHasBeenSetAsDefaultOfType(this IResponsesService responsesService, Contexts contexts, string name)
        {
            return responsesService.ProcessResponse(nameof(ConfigurationValueHasBeenSetAsDefaultOfType), contexts,
                new KeyValuePair<string, string>(nameof(name), name));
        }

        public static string CustomConfigurationHasBeenRemoved(this IResponsesService responsesService, Contexts contexts, string name)
        {
            return responsesService.ProcessResponse(nameof(CustomConfigurationHasBeenRemoved), contexts,
                new KeyValuePair<string, string>(nameof(name), name));
        }

        public static string ServerDoesntHaveCustomValueForConfiguration(this IResponsesService responsesService, Contexts contexts, string name)
        {
            return responsesService.ProcessResponse(nameof(ServerDoesntHaveCustomValueForConfiguration), contexts,
                new KeyValuePair<string, string>(nameof(name), name));
        }
    }
}
