using System.Collections.Generic;

namespace Devscord.DiscordFramework.Framework.Commands.Parsing.Models
{
    public class DiscordRequest
    {
        public string Prefix { get; set; }
        public string Name { get; set; }
        public string ArgumentsPrefix { get; set; }
        public IEnumerable<DiscordRequestArgument> Arguments { get; set; }
        public string OriginalMessage { get; set; }
        public bool IsCommandForBot() => !string.IsNullOrEmpty(this.Prefix);

        public override string ToString()
        {
            return OriginalMessage;
        }

    }
}
