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

            return message;
        }

        private Match FindMatches(string message, params KeyValuePair<string, string>[] values)
        {
            var pattern = new StringBuilder();
            foreach (var param in values)
            {
                pattern.Append($@".*(?<{param.Key}>{{{{{param.Key}}}}}).*");
            }
            return Regex.Match(message, pattern.ToString(), RegexOptions.IgnoreCase);
        }
    }
}
