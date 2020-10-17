using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Settings.Commands.Handlers
{
    public class AddInitEventCommandHandler : ICommandHandler<AddInitEventCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddInitEventCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddInitEventCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            var initEvent = new InitEvent(command.ServerId, command.EndedAt);
            await session.AddAsync(initEvent);
        }
    }
}
