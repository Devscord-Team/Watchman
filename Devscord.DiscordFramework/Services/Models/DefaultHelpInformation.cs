using System.Collections.Generic;

namespace Devscord.DiscordFramework.Services.Models
{
    public class DefaultHelpInformation : Entity
    {
        //public ulong HelpId { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> MethodNames { get; set; }
        public IEnumerable<Description> Descriptions { get; set; }
        public string DefaultDescriptionName { get; set; }

        //public DefaultHelpInformation(ulong helpId) => HelpId = helpId;
    }
}