using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Commands.Handlers
{
    public class AddPreGeneratedStatisticCommandHandler : ICommandHandler<AddPreGeneratedStatisticCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddPreGeneratedStatisticCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddPreGeneratedStatisticCommand command)
        {
            using var session = this._sessionFactory.Create();
            await session.AddAsync(command.PreGeneratedStatistic);
        }
    }
}
