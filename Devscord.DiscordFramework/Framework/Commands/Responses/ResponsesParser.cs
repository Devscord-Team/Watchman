using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands.Responses
{
    public class ResponsesParser
    {
        public string Parse(Response response, IEnumerable<KeyValuePair<string, string>> values)
        {
            var message = response.Message;
            var replaced = this.FindAndReplaceMatches(message, values);
            return replaced;
        }

        private string FindAndReplaceMatches(string message, IEnumerable<KeyValuePair<string, string>> values)
        {
            foreach (var param in values)
            {
                var match = Regex.Match(message, $@".*(?<{param.Key}>{{{{{param.Key}}}}}).*", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                var group = match.Groups[param.Key];
                message = message.Replace(group.Value, param.Value);
            }
            return message;
        }
    }
}
