using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetMessagesStatisticsQuery : PaginationQuery, IQuery<GetMessagesStatisticsQueryResult>
    {
        public Period Period { get; private set; }

        public GetMessagesStatisticsQuery(Period period)
        {
            this.Period = period;
        }
    }
}
