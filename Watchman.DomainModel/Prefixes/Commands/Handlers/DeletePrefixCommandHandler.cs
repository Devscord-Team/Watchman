﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.ServerPrefixes.Commands.Handlers
{
    public class DeletePrefixCommandHandler : ICommandHandler<DeletePrefixCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public DeletePrefixCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(DeletePrefixCommand command)
        {
            using var session = this._sessionFactory.Create();
            await session.DeleteAsync(command.Prefix);
        }
    }
}
