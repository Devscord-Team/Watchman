using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Framework.Commands.Parsing.Models
{
    public class DiscordRequest
    {
        public string Prefix { get; set; }
        public string Name { get; set; }
        public string ArgumentsPrefix { get; set; }
        public IEnumerable<DiscordRequestArgument> Arguments { get; set; }
    }
}
