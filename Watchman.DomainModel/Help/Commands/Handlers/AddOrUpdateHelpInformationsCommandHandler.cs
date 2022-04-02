using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Help.Commands.Handlers
{
    public class AddOrUpdateHelpInformationsCommandHandler : ICommandHandler<AddOrUpdateHelpInformationsCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddOrUpdateHelpInformationsCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddOrUpdateHelpInformationsCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            foreach (var item in command.HelpInformation)
            {
                await session.AddOrUpdateAsync(item);
            }
        }
    }
}
