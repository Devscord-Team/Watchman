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

        public override Task HandleAsync(SetRoleAsSafeCommand command)
        {
            using var session = this._sessionFactory.Create();
            var roles = session.Get<SafeRole>()
                .Where(x => x.ServerId == command.ServerId);

            if (roles.Any(x => x.RoleId == command.RoleId))
            {
                return Task.CompletedTask;
            }
            var role = new SafeRole(command.RoleId, command.ServerId);
            return session.AddAsync(role);
        }
    }
}
