using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Users.Commands.Handlers
{
    public class AddProtectionPunishmentCommandHandler : ICommandHandler<AddProtectionPunishmentCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddProtectionPunishmentCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddProtectionPunishmentCommand command)
        {
            using var session = _sessionFactory.Create();
            await session.AddAsync(command.ProtectionPunishment);
        }
    }
}
