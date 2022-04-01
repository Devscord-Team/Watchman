using Devscord.DiscordFramework.Middlewares.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Commands.Responses
{
    public static class ContextsToResponseFields
    {
        public static IEnumerable<KeyValuePair<string, string>> ConvertToResponseFields(this Contexts contexts, IEnumerable<string> requiredFields)
        {
            var fields = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("context_user", contexts.User.Name),
                new KeyValuePair<string, string>("context_channel", contexts.Channel.Name),
                new KeyValuePair<string, string>("context_server", contexts.Server.Name)
            };
            return fields.Where(x => requiredFields.Contains(x.Key));
        }
    }
}
