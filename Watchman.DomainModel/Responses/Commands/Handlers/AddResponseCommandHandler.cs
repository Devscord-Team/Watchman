using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses.Commands.Handlers
{
    public class AddResponseCommandHandler : ICommandHandler<AddResponseCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddResponseCommandHandler(ISessionFactory sessionFactory) => this._sessionFactory = sessionFactory;

        public async Task HandleAsync(AddResponseCommand command)
        {
            using var session = this._sessionFactory.Create();
            await session.AddAsync(command.Response);
        }
    }
}
