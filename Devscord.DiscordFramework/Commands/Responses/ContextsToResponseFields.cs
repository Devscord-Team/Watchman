using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Commands.Responses
{
    internal static class ContextsToResponseFields
    {
        internal static IEnumerable<KeyValuePair<string, string>> ConvertToResponseFields(this Contexts contexts, IEnumerable<string> requiredFields)
        {
            var fields = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("user", contexts.User.Name),
                new KeyValuePair<string, string>("channel", contexts.Channel.Name),
                new KeyValuePair<string, string>("server", contexts.Server.Name)
            };
            return fields.Where(x => requiredFields.Contains(x.Key));
        }
    }
}
