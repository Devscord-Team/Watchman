using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses.Commands.Handlers
{
    public class AddResponsesCommandHandler : ICommandHandler<AddResponsesCommand>
    {
        private readonly ISessionFactory sessionFactory;

        public AddResponsesCommandHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public Task HandleAsync(AddResponsesCommand command)
        {
            using (var session = sessionFactory.Create())
            {
                session.Add(command.Responses);
            }
            return Task.CompletedTask;
        }
    }
}
