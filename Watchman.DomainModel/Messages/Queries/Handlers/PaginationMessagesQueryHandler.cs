using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.Commons.Queries.Handlers;

namespace Watchman.DomainModel.Messages.Queries.Handlers
{
    public abstract class PaginationMessagesQueryHandler : PaginationQueryHandler
    {
        public IEnumerable<T> Paginate<T>(PaginationMessagesQuery paginationQuery, IEnumerable<T> collection) where T : Message
        {
            collection = base.Paginate(paginationQuery, collection);
            if (paginationQuery.SentDate != null)
            {
                var timeRange = paginationQuery.SentDate;
                collection = collection.Where(x => x.SentAt >= timeRange.Start && x.SentAt <= timeRange.End);
            }
            return collection;
        }

        public IEnumerable<ServerDayStatistic> Paginate(PaginationMessagesQuery paginationQuery, IEnumerable<ServerDayStatistic> collection)
        {
            collection = base.Paginate(paginationQuery, collection);
            if (paginationQuery.SentDate != null)
            {
                var timeRange = paginationQuery.SentDate;
                collection = collection.Where(x => x.Date >= timeRange.Start && x.Date <= timeRange.End);
            }
            return collection;
        }
    }
}
