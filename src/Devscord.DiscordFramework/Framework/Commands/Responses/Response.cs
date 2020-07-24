using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands.Responses
{
    public class Response
    {
        private readonly Regex exField = new Regex(@"{{(?<field>.*?)}}", RegexOptions.Singleline | RegexOptions.Compiled);

        public string OnEvent { get; set; }
        public string Message { get; set; }

        public IEnumerable<string> GetFields()
        {
            var fields = this.exField.Matches(this.Message).Cast<Match>().Select(x => x.Groups["field"].Value);
            return fields;
        }
    }

}
