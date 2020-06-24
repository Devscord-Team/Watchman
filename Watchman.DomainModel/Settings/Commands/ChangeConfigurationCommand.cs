using Watchman.Cqrs;

namespace Watchman.DomainModel.Settings.Commands
{
    internal class ChangeConfigurationCommand : ICommand
    {
        public Configuration Configuration { get; }

        public ChangeConfigurationCommand(Configuration configuration)
        {
            this.Configuration = configuration;
        }
    }
}
