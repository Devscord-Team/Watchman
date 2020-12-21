using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.DiscordServer.Commands.Handlers
{
    public class SetRoleAsUntrustedCommandHandler : ICommandHandler<SetRoleAsUntrustedCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public SetRoleAsUntrustedCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public Task HandleAsync(SetRoleAsUntrustedCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            var trustedRoles = session.Get<TrustedRole>().Where(x => x.ServerId == command.ServerId);
            var trustedRole = trustedRoles.FirstOrDefault(x => x.RoleId == command.RoleId);
            return trustedRole == null
                ? Task.CompletedTask
                : session.DeleteAsync(trustedRole);
        }
    }
}
