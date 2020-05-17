using System;
using System.Collections.Generic;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help
{
    public class HelpInformation : Entity
    {
        public const ulong DEFAULT_SERVER_INDEX = 0;

        public Guid HelpId { get; set; }
        public IEnumerable<string> Names { get; set; }
        public string MethodFullName { get; set; }
        public IEnumerable<ArgumentInfo> ArgumentInfos { get; set; }
        public IEnumerable<Description> Descriptions { get; set; }
        public ulong ServerId { get; set; }
        public bool IsDefault => ServerId == DEFAULT_SERVER_INDEX;

        public HelpInformation()
        {
            HelpId = Guid.NewGuid();
        }
    }
}