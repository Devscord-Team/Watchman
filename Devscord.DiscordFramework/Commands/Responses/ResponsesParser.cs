using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Commands.Responses
{
    public class ResponsesParser
    {
        public string Parse(Response response, IEnumerable<KeyValuePair<string, string>> values)
        {
            var message = response.Message;
            foreach (var param in values)
            {
                var match = Regex.Match(message, $@".*(?<{param.Key}>{{{{{param.Key}}}}}).*", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                var group = match.Groups[param.Key];
                message = message.Replace(group.Value, param.Value);
            }
            return message.Replace(@"\n", "\n").Replace(@"\r", "\r");
        }
    }
}
