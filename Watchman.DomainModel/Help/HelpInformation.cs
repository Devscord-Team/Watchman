using System.Collections.Generic;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help
{
    public class HelpInformation : Entity
    {
        public const ulong EMPTY_SERVER_ID = 0;

        public string CommandName { get; set; }
        public IEnumerable<ArgumentInformation> ArgumentInformations { get; set; }
        public IEnumerable<Description> Descriptions { get; set; }
        public ulong ServerId { get; set; }
        public string DefaultLanguage { get; set; } = "EN";
        public bool IsDefault => this.ServerId == EMPTY_SERVER_ID;
    }
}