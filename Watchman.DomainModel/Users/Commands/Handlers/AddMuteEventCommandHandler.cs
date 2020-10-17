using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Users.Commands.Handlers
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
            using var session = this._sessionFactory.CreateMongo();
            await session.AddAsync(command.MuteEvent);
        }
    }
}
