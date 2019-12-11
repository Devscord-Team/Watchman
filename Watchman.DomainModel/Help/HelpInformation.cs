using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help
{
    struct Description
    {
        public string Name { get; }
        public bool IsDefault { get; }
        public string Details { get; }
    }

    class HelpInformation : Entity
    {
        public ulong ServerId { get; }
        public string MethodName { get; }
        public List<Description> Descriptions { get; }
    }
}
