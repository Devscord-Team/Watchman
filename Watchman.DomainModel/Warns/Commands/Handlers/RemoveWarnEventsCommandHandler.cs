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

        public Task HandleAsync(RemoveWarnEventsCommand command)
        {
            if (command.GrantorId == null && command.ReceiverId == null)
            {
                return Task.CompletedTask;
            }
            using var session = this._sessionFactory.Create();
            return session.DeleteAsync<WarnEvent>(x =>
                    (x.ServerId == command.ServerId)
                    && (command.ReceiverId == null || command.ReceiverId == x.ReceiverId)
                    && (command.GrantorId == null || command.GrantorId == x.GrantorId)
                    && (x.CreatedAt >= command.From)
                );
        }
    }
}
