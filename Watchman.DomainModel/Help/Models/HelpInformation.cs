using System;
using System.Collections.Generic;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help.Models
{
    public class HelpInformation : Entity
    {
        public Guid HelpId { get; set; }
        public string Prefix { get; set; }
        public IEnumerable<string> Names { get; set; }
        public string MethodName { get; set; }
        public IEnumerable<ArgumentInfo> ArgumentInfos { get; set; }
        public IEnumerable<Description> Descriptions { get; set; }
        public string DefaultDescriptionName { get; set; }
        public ulong ServerId { get; set; }
        public bool IsDefault => ServerId == 0;

        public HelpInformation()
        {
            HelpId = Guid.NewGuid();
        }
    }
}