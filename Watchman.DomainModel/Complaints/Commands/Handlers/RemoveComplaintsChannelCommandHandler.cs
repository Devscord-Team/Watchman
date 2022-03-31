using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Complaints.Commands.Handlers
{
    public class RemoveComplaintsChannelCommandHandler : ICommandHandler<RemoveComplaintsChannelCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public RemoveComplaintsChannelCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public Task HandleAsync(RemoveComplaintsChannelCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            return session.DeleteAsync(command.ComplaintsChannel); //todo: use new way of deleting by filter
        }
    }
}
