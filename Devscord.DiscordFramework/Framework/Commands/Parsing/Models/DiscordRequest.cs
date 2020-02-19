using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Framework.Commands.Parsing.Models
{
    public class DiscordRequest
    {
        public string Prefix { get; set; }
        public string Name { get; set; }
        public IEnumerable<DiscordRequestArgument> Arguments { get; set; }
        public string OriginalMessage { get; set; }
        public bool IsCommandForBot => !string.IsNullOrEmpty(this.Prefix);

        public bool HasArgument(string name)
        {
            return Arguments.Any(x => x.Name == name);
        }

        public bool HasArgument(string name, string value)
        {
            return string.IsNullOrEmpty(name)
                ? Arguments.Any(a => a.Value == value)
                : this.Arguments.Any(x => x.Name == name && x.Value == value);
        }

        public override string ToString()
        {
            return OriginalMessage;
        }
    }
}
