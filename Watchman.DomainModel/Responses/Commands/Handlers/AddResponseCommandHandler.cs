using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Responses.Commands.Handlers
{
    public class AddResponseCommandHandler : ICommandHandler<AddResponseCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddResponseCommandHandler(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddResponseCommand command)
        {
            using var session = _sessionFactory.Create();
            await session.AddAsync(command.Response);
        }
    }
}
