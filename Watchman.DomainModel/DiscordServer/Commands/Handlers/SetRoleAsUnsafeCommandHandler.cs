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

        public override Task HandleAsync(SetRoleAsUnsafeCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            var role = session.Get<SafeRole>()
                .FirstOrDefault(x => x.ServerId == command.ServerId && x.RoleId == command.RoleId);

            return role == null ? Task.CompletedTask : session.DeleteAsync(role);
        }
    }
}
