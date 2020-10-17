using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.DiscordServer.Commands.Handlers
{
    public abstract class UpdateSafetyOfRoleCommandHandler<T> : ICommandHandler<T> where T : UpdateSafetyOfRoleCommand
    {
        protected readonly ISessionFactory _sessionFactory;

        protected UpdateSafetyOfRoleCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public abstract Task HandleAsync(T command);
    }
}
