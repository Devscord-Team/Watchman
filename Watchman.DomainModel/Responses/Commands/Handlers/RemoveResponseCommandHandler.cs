using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses.Commands.Handlers
{
    public class RemoveResponseCommandHandler : ICommandHandler<RemoveResponseCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public RemoveResponseCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }
        public async Task HandleAsync(RemoveResponseCommand command)
        {
            using var session = _sessionFactory.Create();
            var onEvent = session.Get<Response>()
                .FirstOrDefault(x => x.ServerId == command.ServerId && x.OnEvent == command.OnEvent);
            if (onEvent == null)
            {
                return;
            }
            await session.DeleteAsync(onEvent);
        }
    }
}