using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Mute.Commands.Handlers
{
    public class AddMuteInfoToDbCommandHandler : ICommandHandler<AddMuteInfoToDbCommand>
    {
        private readonly ISession _session;

        public AddMuteInfoToDbCommandHandler(ISessionFactory sessionFactory)
        {
            this._session = sessionFactory.Create();
        }

        public Task HandleAsync(AddMuteInfoToDbCommand command)
        {
            _session.Add(command.MuteEvent);
            return Task.CompletedTask;
        }
    }
}
