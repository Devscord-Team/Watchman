using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Mute.Commands.Handlers
{
    public class AddMuteEventCommandHandler : ICommandHandler<AddMuteEventCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddMuteEventCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddMuteEventCommand command)
        {
            using var session = _sessionFactory.Create();
            await Task.Run(() => session.Add(command.MuteEvent));
        }
    }
}
