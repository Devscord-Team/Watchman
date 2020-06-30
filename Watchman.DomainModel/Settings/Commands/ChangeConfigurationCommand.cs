using Watchman.Cqrs;

namespace Watchman.DomainModel.Settings.Commands
{
    internal class ChangeConfigurationCommand : ICommand
    {
        public IConfigurationItem ConfigurationItem { get; }

        public ChangeConfigurationCommand(IConfigurationItem configurationItem)
        {
            this.ConfigurationItem = configurationItem;
        }
    }
}
