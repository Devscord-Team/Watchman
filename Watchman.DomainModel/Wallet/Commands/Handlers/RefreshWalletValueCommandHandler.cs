using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.DomainModel.Wallet.Services;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet.Commands.Handlers
{
    public class RefreshWalletValueCommandHandler : ICommandHandler<RefreshWalletValueCommand>
    {
        private readonly ISessionFactory sessionFactory;
        private readonly RefreshingWalletService refreshingWalletService;

        public RefreshWalletValueCommandHandler(ISessionFactory sessionFactory, RefreshingWalletService refreshingWalletService)
        {
            this.sessionFactory = sessionFactory;
            this.refreshingWalletService = refreshingWalletService;
        }

        public Task HandleAsync(RefreshWalletValueCommand command)
        {
            using var session = this.sessionFactory.CreateMongo();
            return this.refreshingWalletService.RefreshWallet(session, command.ServerId, command.UserId);
        }
    }
}
