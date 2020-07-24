using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Commands
{
    public class AddHelpInformationCommand : ICommand
    {
        public IEnumerable<HelpInformation> HelpInformation { get; private set; }

        public AddHelpInformationCommand(IEnumerable<HelpInformation> helpInformation)
        {
            this.HelpInformation = helpInformation;
        }
    }
}
