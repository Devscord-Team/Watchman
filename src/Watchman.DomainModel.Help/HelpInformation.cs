using System;
using System.Collections.Generic;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help
{
    public class HelpInformation : Entity, IAggregateRoot //TODO refactoring
    {
        public const ulong EMPTY_SERVER_ID = 0;

        public Guid HelpId { get; private set; }
        public IEnumerable<string> Names { get; private set; }
        public string MethodFullName { get; private set; }
        public IEnumerable<ArgumentInfo> ArgumentInfos { get; private set; }
        public IEnumerable<Description> Descriptions { get; private set; }
        public ulong ServerId { get; private set; }
        public bool IsDefault => this.ServerId == EMPTY_SERVER_ID;

        public HelpInformation()
        {
            this.HelpId = Guid.NewGuid();
        }
    }
}