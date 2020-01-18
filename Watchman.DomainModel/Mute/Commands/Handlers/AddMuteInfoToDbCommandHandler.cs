using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Mute.Commands.Handlers
{
    public class AddMuteInfoToDbCommandHandler : ICommandHandler<AddMuteInfoToDbCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddMuteInfoToDbCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public Task HandleAsync(AddMuteInfoToDbCommand command)
        {
            using var session = _sessionFactory.Create();
            session.Add(command.MuteEvent);
            return Task.CompletedTask;
        }
    }
}
