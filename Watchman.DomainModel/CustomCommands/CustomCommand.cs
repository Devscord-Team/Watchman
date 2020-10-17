using System.Text.RegularExpressions;

using Watchman.Integrations.Database;

namespace Watchman.DomainModel.CustomCommands
{
    public class CustomCommand : Entity, IAggregateRoot
    {
        public string CommandFullName { get; private set; }
        public string CustomTemplateRegex { get; private set; }
        public ulong ServerId { get; private set; }

        public CustomCommand(string commandFullName, string customTemplateRegex, ulong serverId)
        {
            this.CommandFullName = commandFullName;
            this.CustomTemplateRegex = customTemplateRegex;
            this.ServerId = serverId;
        }

        public Regex GetTemplate()
        {
            return new Regex(this.CustomTemplateRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }
}
