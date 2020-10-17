using System.Linq;
using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Responses.Commands.Handlers
{
    public class AddOrUpdateResponsesCommandHandler : ICommandHandler<AddOrUpdateResponsesCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddOrUpdateResponsesCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddOrUpdateResponsesCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            foreach (var response in command.ResponsesToAddOrUpdate)
            {
                await session.AddOrUpdateAsync(response);
            }
        }
    }
}
