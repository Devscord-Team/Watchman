using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings.Commands.Handlers
{
    internal class ChangeConfigurationCommandHandler : ICommandHandler<ChangeConfigurationCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public ChangeConfigurationCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(ChangeConfigurationCommand command)
        {
            //var session = this._sessionFactory.Create();
            //await session.AddOrUpdateAsync(command.ConfigurationItem);
        }
    }
}
