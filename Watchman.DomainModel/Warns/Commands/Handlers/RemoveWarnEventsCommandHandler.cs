using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Warns.Commands.Handlers
{
    public class RemoveWarnEventsCommandHandler : ICommandHandler<RemoveWarnEventsCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public RemoveWarnEventsCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(RemoveWarnEventsCommand command)
        {
            using var session = this._sessionFactory.Create();
            await session.DeleteAsync<WarnEvent>(x =>
                (command.GrantorId == null || command.GrantorId == x.GrantorId) &&
                (command.ReceiverId == null || command.ReceiverId == x.ReceiverId) &&
                (command.Reason == null || command.Reason == x.Reason) &&
                (x.ServerId == command.ServerId));
        }
    }
}
