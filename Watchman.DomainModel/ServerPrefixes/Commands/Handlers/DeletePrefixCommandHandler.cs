using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Exceptions;
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
            var prefixes = session.Get<ServerPrefixes>().FirstOrDefault(x => x.ServerId == command.ServerId);
            if (prefixes == null)
            {
                prefixes = new ServerPrefixes(command.ServerId);
            }
            var version = prefixes.Version;
            prefixes.DeletePrefix(command.Prefix);
            if(prefixes.Version == version)
            {
                throw new NotUpdatedException();
            }
            await session.AddOrUpdateAsync(prefixes);
        }
    }
}
