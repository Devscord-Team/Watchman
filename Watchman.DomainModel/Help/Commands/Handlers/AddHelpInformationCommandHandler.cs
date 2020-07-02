using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help.Commands.Handlers
{
    public class AddHelpInformationCommandHandler : ICommandHandler<AddHelpInformationCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddHelpInformationCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddHelpInformationCommand command)
        {
            using var session = this._sessionFactory.Create();
            await session.AddAsync(command.HelpInformation);
        }
    }
}
