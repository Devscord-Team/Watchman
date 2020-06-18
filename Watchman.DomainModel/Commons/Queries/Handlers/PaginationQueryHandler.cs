using System.Collections.Generic;
using System.Linq;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Commons.Queries.Handlers
{
    public abstract class PaginationQueryHandler
    {
        public IEnumerable<T> Paginate<T>(PaginationQuery<T> paginationQuery, IEnumerable<T> collection) where T : Entity
        {
            if (paginationQuery.Filter != null)
            {
                collection = collection.Where(paginationQuery.Filter);
            }
            return this.Paginate((PaginationQuery)paginationQuery, collection);
        }

        public IEnumerable<T> Paginate<T>(PaginationQuery paginationQuery, IEnumerable<T> collection) where T : Entity
        {
            if (paginationQuery.CreatedDate != null)
            {
                var timeRange = paginationQuery.CreatedDate;
                collection = collection.Where(x => x.CreatedAt >= timeRange.Start && x.CreatedAt <= timeRange.End);
            }
            if (paginationQuery.UpdatedDate != null)
            {
                var timeRange = paginationQuery.UpdatedDate;
                collection = collection.Where(x => x.UpdatedAt >= timeRange.Start && x.UpdatedAt <= timeRange.End);
            }
            if (paginationQuery.Skip.HasValue)
            {
                collection = collection.Skip(paginationQuery.Skip.Value);
            }
            if (paginationQuery.Quantity.HasValue)
            {
                collection = collection.Take(paginationQuery.Quantity.Value);
            }
            return collection;
        }
    }
}
