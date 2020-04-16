using System.Linq;
using System.Threading.Tasks;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.DiscordServer.Commands.Handlers
{
    public class SetRoleAsSafeCommandHandler : UpdateSafetyOfRoleCommandHandler<SetRoleAsSafeCommand>
    {
        public SetRoleAsSafeCommandHandler(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public override async Task HandleAsync(SetRoleAsSafeCommand command)
        {
            using var session = _sessionFactory.Create();
            var roles = session.Get<Role>()
                .Where(x => x.ServerId == command.ServerId);
            if (roles.Any(x => x.Name == command.RoleName))
            {
                return;
            }

            var role = new Role(command.RoleName, command.ServerId);
            await session.AddAsync(role);
        }
    }
}
