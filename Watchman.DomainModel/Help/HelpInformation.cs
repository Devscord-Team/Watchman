using System.Collections.Generic;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help
{
    public class HelpInformation : Entity
    {
        public const ulong EMPTY_SERVER_ID = 0;

        public string CommandName { get; private set; }
        public string AreaName { get; private set; }
        public IEnumerable<ArgumentInformation> ArgumentInformations { get; private set; }
        public IEnumerable<Description> Descriptions { get; private set; }
        public ulong ServerId { get; private set; }
        public string DefaultLanguage { get; private set; } = "EN";
        public string ExampleUsage { get; private set; }
        public bool IsDefault => this.ServerId == EMPTY_SERVER_ID;

        public HelpInformation(string commandName, string areaName, IEnumerable<ArgumentInformation> argumentInformations, IEnumerable<Description> descriptions, ulong serverId)
        {
            this.CommandName = commandName;
            this.AreaName = areaName;
            this.ArgumentInformations = argumentInformations;
            this.Descriptions = descriptions;
            this.ServerId = serverId;
        }

        public HelpInformation SetCommandName(string commandName)
        {
            this.CommandName = commandName;
            this.Update();
            return this;
        }

        public HelpInformation SetAreaName(string areaName)
        {
            this.AreaName = areaName;
            this.Update();
            return this;
        }

        public HelpInformation SetArgumentInformations(IEnumerable<ArgumentInformation> argumentInformations)
        {
            this.ArgumentInformations = argumentInformations;
            this.Update();
            return this;
        }

        public HelpInformation SetDescriptions(IEnumerable<Description> descriptions)
        {
            this.Descriptions = descriptions;
            this.Update();
            return this;
        }

        public HelpInformation SetServerId(ulong serverId)
        {
            this.ServerId = serverId;
            this.Update();
            return this;
        }

        public HelpInformation SetDefaultLanguage(string defaultLanguage)
        {
            this.DefaultLanguage = defaultLanguage;
            this.Update();
            return this;
        }

        public HelpInformation SetExampleUsage(string exampleUsage)
        {
            this.ExampleUsage = exampleUsage;
            this.Update();
            return this;
        }
    }
}