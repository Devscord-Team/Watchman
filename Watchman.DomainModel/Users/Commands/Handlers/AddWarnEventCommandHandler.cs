using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Users.Commands.Handlers
{
    public class AddWarnEventCommandHandler : ICommandHandler<AddWarnEventCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddWarnEventCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddWarnEventCommand command)
        {
            using var session = this._sessionFactory.Create();
            await session.AddAsync(command.WarnEvent);
        }
    }
}
