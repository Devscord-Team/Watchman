using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet.Commands.Handlers
{
    public class InitializeWalletForUserCommandHandler : ICommandHandler<InitializeWalletForUserCommand>
    {
        private readonly ISessionFactory sessionFactory;

        public InitializeWalletForUserCommandHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public Task HandleAsync(InitializeWalletForUserCommand command)
        {
            var session = this.sessionFactory.CreateMongo();
            var wallet = new Wallet(command.ServerId, command.UserId);
            return session.AddAsync(wallet);
        }
    }
}
