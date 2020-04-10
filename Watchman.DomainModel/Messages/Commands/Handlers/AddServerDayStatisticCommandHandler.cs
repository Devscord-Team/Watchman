using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Commands.Handlers
{
    public class AddServerDayStatisticCommandHandler : ICommandHandler<AddServerDayStatisticCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddServerDayStatisticCommandHandler(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }
        public async Task HandleAsync(AddServerDayStatisticCommand command)
        {
            using var session = _sessionFactory.Create();
            session.Add(command.ServerDayStatistic);
        }
    }
}
