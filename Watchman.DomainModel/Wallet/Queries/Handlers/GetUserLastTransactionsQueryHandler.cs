using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Cqrs;
using Watchman.DomainModel.Wallet.ValueObjects;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Wallet.Queries.Handlers
{
    public class GetUserLastTransactionsQueryHandler : IQueryHandler<GetUserLastTransactionsQuery, GetUserLastTransactionsQueryResult>
    {
        private readonly ISessionFactory sessionFactory;

        public GetUserLastTransactionsQueryHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public GetUserLastTransactionsQueryResult Handle(GetUserLastTransactionsQuery query)
        {
            using var session = this.sessionFactory.CreateMongo();
            var transactions = session.Get<WalletTransaction>()
                .Where(x => x.OnServerId == query.ServerId && (x.FromUserId == query.UserId || x.ToUserId == query.UserId))
                .OrderByDescending(x => x.CreatedAt)
                .Take(query.Quantity)
                .ToList();
            return new GetUserLastTransactionsQueryResult(transactions);
        }
    }
}
