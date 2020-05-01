using Devscord.DiscordFramework.Framework.Commands.Properties;
using System.Collections.Generic;

namespace Devscord.DiscordFramework.Framework.Commands
{
    public class BotCommandTemplate
    {
        public string CommandName { get; private set; }
        public string NormalizedCommandName { get; private set; }
        public IEnumerable<BotCommandProperty> Properties { get; private set; }

        public BotCommandTemplate(string commandName, IEnumerable<BotCommandProperty> properties)
        {
            this.CommandName = commandName;
            this.Properties = properties;
            this.NormalizedCommandName = commandName.ToLowerInvariant();
            if (this.NormalizedCommandName.EndsWith("command"))
            {
                this.NormalizedCommandName = this.NormalizedCommandName.Substring(0, this.NormalizedCommandName.Length - "command".Length);
            }
        }
    }
}
