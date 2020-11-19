using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Warns.Commands.Handlers
{
    public class AddWarnEventCommandHandler : ICommandHandler<AddWarnEventCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddWarnEventCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddWarnEventCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            var warnEvent = new WarnEvent(command.ReceiverId, command.GranterId, command.Reason, command.ServerId);
            await session.AddAsync(warnEvent);
        }
    }
}
