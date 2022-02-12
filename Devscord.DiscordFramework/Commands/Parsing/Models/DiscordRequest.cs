using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Framework.Commands.Parsing.Models
{
    public class DiscordRequest //TODO add lazy loading for everything except IsCommandForBot
    {
        public string Prefix { get; set; }
        public string Name { get; set; }
        public IEnumerable<DiscordRequestArgument> Arguments { get; set; }
        public string OriginalMessage { get; set; }
        public bool IsCommandForBot => !string.IsNullOrEmpty(this.Prefix);
        public DateTime SentAt { get; set; }

        public bool HasArgument(string name)
        {
            return this.Arguments.Any(x => x.Name == name);
        }

        public bool HasArgument(string name, string value)
        {
            return string.IsNullOrEmpty(name)
                ? this.Arguments.Any(a => a.Value == value)
                : this.Arguments.Any(x => x.Name == name && x.Value == value);
        }

        public override string ToString()
        {
            return this.OriginalMessage;
        }
    }
}
