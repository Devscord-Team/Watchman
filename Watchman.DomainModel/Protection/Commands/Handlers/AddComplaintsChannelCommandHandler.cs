using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Protection.Commands.Handlers
{
    public class AddComplaintsChannelCommandHandler : ICommandHandler<AddComplaintsChannelCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddComplaintsChannelCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public Task HandleAsync(AddComplaintsChannelCommand command)
        {
            using var session = this._sessionFactory.Create();
            var complaintsChannel = new ComplaintsChannel(command.ChannelId);
            return session.AddAsync(complaintsChannel);
        }
    }
}
