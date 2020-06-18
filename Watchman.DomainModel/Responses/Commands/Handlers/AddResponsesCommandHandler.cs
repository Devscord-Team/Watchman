using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses.Commands.Handlers
{
    public class AddResponsesCommandHandler : ICommandHandler<AddResponsesCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddResponsesCommandHandler(ISessionFactory sessionFactory) => this._sessionFactory = sessionFactory;

        public async Task HandleAsync(AddResponsesCommand command)
        {
            using var session = this._sessionFactory.Create();
            await session.AddAsync(command.Responses);
        }
    }
}
