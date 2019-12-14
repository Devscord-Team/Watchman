using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands.Responses
{
    public class ResponsesParser
    {
        public string Parse(Response response, params KeyValuePair<string, string>[] values)
        {
            var message = response.Message;
            var match = this.FindMatches(message, values);
            var replaced = this.ReplaceMatched(message, match, values);
            return replaced;
        }

        private Match FindMatches(string message, KeyValuePair<string, string>[] values)
        {
            var pattern = new StringBuilder();
            foreach (var param in values)
            {
                pattern.Append($@".*(?<{param.Key}>{{{{{param.Key}}}}}).*");
            }
            return Regex.Match(message, pattern.ToString(), RegexOptions.IgnoreCase);
        }

        private string ReplaceMatched(string message, Match match, KeyValuePair<string, string>[] values)
        {
            foreach (var param in values)
            {
                var group = match.Groups[param.Key];
                message = message.Replace(group.Value, param.Value);
            }
            return message;
        }
    }
}
