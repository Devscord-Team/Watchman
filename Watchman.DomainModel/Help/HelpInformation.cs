using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help
{
    public struct Description
    {
        public string Name { get; }
        public bool IsDefault { get; }
        public string Details { get; }
    }

    public class HelpInformation : Entity
    {
        public ulong ServerId { get; }
        public string MethodName { get; }
        public List<Description> Descriptions { get; }
    }
}
