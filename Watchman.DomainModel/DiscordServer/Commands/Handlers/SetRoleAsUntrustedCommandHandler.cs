using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.DiscordServer.Commands.Handlers
{
    public class SetRoleAsUntrustedCommandHandler : ICommandHandler<SetRoleAsUntrustedCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public SetRoleAsUntrustedCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(SetRoleAsUntrustedCommand command)
        {
            using var session = this._sessionFactory.Create();
            var trustedRoles = session.Get<TrustedRole>().Where(x => x.ServerId == command.ServerId);
            var trustedRole = trustedRoles.FirstOrDefault(x => x.RoleId == command.RoleId);
            if (trustedRole == null)
            {
                return;
            }
            await session.DeleteAsync(trustedRole);
        }
    }
}
