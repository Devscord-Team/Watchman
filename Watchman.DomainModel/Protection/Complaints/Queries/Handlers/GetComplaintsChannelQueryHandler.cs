using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Protection.Complaints.Queries.Handlers
{
    public class GetComplaintsChannelQueryHandler : IQueryHandler<GetComplaintsChannelQuery, GetComplaintsChannelQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetComplaintsChannelQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetComplaintsChannelQueryResult Handle(GetComplaintsChannelQuery query)
        {
            using var session = this._sessionFactory.CreateMongo();
            var complaintsChannel = session.Get<ComplaintsChannel>().FirstOrDefault(x => x.ServerId == query.ServerId);
            return new GetComplaintsChannelQueryResult(complaintsChannel);
        }
    }
}
