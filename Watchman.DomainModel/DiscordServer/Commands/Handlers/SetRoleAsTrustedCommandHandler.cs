using System.Linq;
using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.DiscordServer.Commands.Handlers
{
    public class SetRoleAsTrustedCommandHandler : ICommandHandler<SetRoleAsTrustedCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public SetRoleAsTrustedCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(SetRoleAsTrustedCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            var trustedRoles = session.Get<TrustedRole>().Where(x => x.ServerId == command.ServerId);
            if (trustedRoles.Any(x => x.RoleId == command.RoleId))
            {
                return;
            }
            var trustedRole = new TrustedRole(command.RoleId, command.ServerId);
            await session.AddAsync(trustedRole);
        }
    }
}
