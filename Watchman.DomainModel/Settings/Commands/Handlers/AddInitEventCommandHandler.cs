using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings.Commands.Handlers
{
    public class AddInitEventCommandHandler : ICommandHandler<AddInitEventCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddInitEventCommandHandler(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddInitEventCommand command)
        {
            using var session = _sessionFactory.Create();
            var initEvent = new InitEvent(command.ServerId, command.EndedAt);
            await session.AddAsync(initEvent);
        }
    }
}
