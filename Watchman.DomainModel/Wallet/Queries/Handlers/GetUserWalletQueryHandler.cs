using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet.Queries.Handlers
{
    public class GetUserWalletQueryHandler : IQueryHandler<GetUserWalletQuery, GetUserWalletQueryResult>
    {
        private readonly ISessionFactory sessionFactory;

        public GetUserWalletQueryHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public GetUserWalletQueryResult Handle(GetUserWalletQuery query)
        {
            using var session = this.sessionFactory.CreateMongo();
            var wallet = session.Get<Wallet>().FirstOrDefault(x => x.ServerId == query.ServerId && x.UserId == query.UserId);
            return new GetUserWalletQueryResult(wallet);
        }
    }
}
