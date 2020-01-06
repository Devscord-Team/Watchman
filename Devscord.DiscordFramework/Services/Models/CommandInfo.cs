using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Services.Models
{
    public class CommandInfo
    {
        public string Prefix { get; set; }
        public IEnumerable<string> Names { get; set; }
        public string MethodName { get; set; }
        public IEnumerable<CommandArgumentInfo> CommandArgumentInfos { get; set; }
    }
}
