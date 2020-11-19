using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Responses.Commands.Handlers
{
    public class UpdateResponseCommandHandler : ICommandHandler<UpdateResponseCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public UpdateResponseCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(UpdateResponseCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            var response = session.Get<Response>(command.Id);
            response.SetMessage(command.Message);
            await session.UpdateAsync(response);
        }
    }
}
