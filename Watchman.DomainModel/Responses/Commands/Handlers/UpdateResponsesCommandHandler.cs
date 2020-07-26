using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses.Commands.Handlers
{
    public class UpdateResponsesCommandHandler : ICommandHandler<UpdateResponsesCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public UpdateResponsesCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(UpdateResponsesCommand command)
        {
            using var session = this._sessionFactory.Create();
            var responses = session.Get<Response>();
            foreach (var response in command.ResponsesToUpdate)
            {
                await session.UpdateAsync(response);
            }

            if (command.ResponsesToAdd != null && command.ResponsesToAdd.Any())
            {
                await session.AddAsync(command.ResponsesToAdd);
            }
        }
    }
}
