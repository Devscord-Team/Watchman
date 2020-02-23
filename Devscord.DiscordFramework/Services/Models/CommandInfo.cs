using System.Collections.Generic;

namespace Devscord.DiscordFramework.Services.Models
{
    public class CommandInfo
    {
        public IEnumerable<string> Names { get; set; }
        public string MethodFullName { get; set; }
        public IEnumerable<CommandArgumentInfo> CommandArgumentInfos { get; set; }
    }
}
