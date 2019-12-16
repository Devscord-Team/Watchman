using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help
{
    public class Description
    {
        public string Name { get; set; }
        public string Details { get; set; }
    }

    public class DefaultHelpInformation : Entity
    {
        public ulong HelpId { get; set; }
        public IEnumerable<string> MethodNames { get; set; }
        public IEnumerable<Description> Descriptions { get; set; }
        public string DefaultDescriptionName { get; set; }

        private static ulong _lastId;
        public DefaultHelpInformation() => HelpId = _lastId++;
    }

    public class ServerHelpInformation : DefaultHelpInformation
    {
        public ulong ServerId { get; set; }
    }
}
