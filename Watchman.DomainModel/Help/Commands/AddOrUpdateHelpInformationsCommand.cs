using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Commands
{
    public class AddOrUpdateHelpInformationsCommand : ICommand
    {
        public IEnumerable<HelpInformation> HelpInformation { get; private set; }

        public AddOrUpdateHelpInformationsCommand(IEnumerable<HelpInformation> helpInformation)
        {
            this.HelpInformation = helpInformation;
        }
    }
}
