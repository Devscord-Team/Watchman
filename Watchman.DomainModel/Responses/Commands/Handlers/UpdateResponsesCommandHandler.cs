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
            foreach (var response in command.Responses)
            {
                var existingResponse = responses.FirstOrDefault(dbResponse => dbResponse.OnEvent == response.OnEvent);
                if (existingResponse == null)
                {
                    // there's no response with supplied onEvent in the database, add a new one
                    await session.AddAsync(response);
                }
                else
                {
                    // there's a response with supplied onEvent in the database, update existing one
                    response.Id = existingResponse.Id;
                    
                }
            }
        }
    }
}
