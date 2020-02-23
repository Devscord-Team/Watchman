using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses.Commands.Handlers
{
    public class UpdateResponseCommandHandler : ICommandHandler<UpdateResponseCommand>
    {
        private readonly ISessionFactory sessionFactory;

        public UpdateResponseCommandHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public Task HandleAsync(UpdateResponseCommand command)
        {
            using (var session = sessionFactory.Create())
            {
                var response = session.Get<Response>(command.Id);
            }
        }
    }
}
