using System;
using System.Collections.Generic;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help
{
    public class HelpInformation : Entity
    {
        public Guid HelpId { get; set; }
        public string Prefix { get; set; } //todo delete, we don't need it
        public IEnumerable<string> Names { get; set; }
        public string MethodName { get; set; } //TODO there should be .FullName instead .Name, because name without class name and namespace is useless
        public IEnumerable<ArgumentInfo> ArgumentInfos { get; set; }
        public IEnumerable<Description> Descriptions { get; set; }
        public string DefaultDescriptionName { get; set; } //TODO we don't need it, because always default should be element that is first on list
        public ulong ServerId { get; set; }
        public bool IsDefault => ServerId == 0;

        public HelpInformation()
        {
            HelpId = Guid.NewGuid();
        }
    }
}