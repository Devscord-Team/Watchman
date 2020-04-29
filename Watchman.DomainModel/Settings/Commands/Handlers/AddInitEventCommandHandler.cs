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
            var session = _sessionFactory.Create();
            var initEvent = new InitEvent
            {
                ServerId = command.ServerId,
                EndedAt = command.EndedAt
            };
            await session.AddAsync(initEvent);
        }
    }
}
