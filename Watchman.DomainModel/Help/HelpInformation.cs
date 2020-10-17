using System;
using System.Collections.Generic;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Help
{
    public class HelpInformation : Entity
    {
        public const ulong EMPTY_SERVER_ID = 0;

        public Guid HelpId { get; set; }
        public IEnumerable<string> Names { get; set; }
        public string MethodFullName { get; set; }
        public IEnumerable<ArgumentInfo> ArgumentInfos { get; set; }
        public IEnumerable<Description> Descriptions { get; set; }
        public ulong ServerId { get; set; }
        public bool IsDefault => this.ServerId == EMPTY_SERVER_ID;

        public HelpInformation()
        {
            this.HelpId = Guid.NewGuid();
        }
    }
}