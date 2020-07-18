using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.ServerPrefixes.Commands.Handlers
{
    public class AddPrefixCommandHandler : ICommandHandler<AddPrefixCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddPrefixCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddPrefixCommand command)
        {
            using var session = this._sessionFactory.Create();
            var prefixes = session.Get<ServerPrefixes>().FirstOrDefault(x => x.ServerId == command.ServerId);
            if(prefixes == null)
            {
                prefixes = new ServerPrefixes(command.ServerId);
            }
            prefixes.AddPrefix(command.Prefix);
            await session.AddOrUpdateAsync(prefixes);
        }
    }
}
