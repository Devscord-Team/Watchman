using System.Linq;
using System.Threading.Tasks;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.DiscordServer.Commands.Handlers
{
    public class SetRoleAsUnsafeCommandHandler : UpdateSafetyOfRoleCommandHandler<SetRoleAsUnsafeCommand>
    {
        public SetRoleAsUnsafeCommandHandler(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public override async Task HandleAsync(SetRoleAsUnsafeCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            var role = session.Get<Role>()
                .Where(x => x.ServerId == command.ServerId)
                .FirstOrDefault(x => x.Name == command.RoleName);

            if (role == null)
            {
                return;
            }
            await session.DeleteAsync(role);
        }
    }
}
