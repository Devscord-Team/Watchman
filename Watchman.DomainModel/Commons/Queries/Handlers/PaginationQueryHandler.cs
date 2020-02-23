using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Watchman.DomainModel.Commons.Queries.Handlers
{
    public abstract class PaginationQueryHandler
    {
        public IEnumerable<T> Paginate<T>(PaginationQuery<T> paginationQuery, IEnumerable<T> collection)
        {
            if(paginationQuery.Filter != null)
            {
                collection = collection.Where(paginationQuery.Filter);
            }
            return this.Paginate((PaginationQuery)paginationQuery, collection);
        }

        public IEnumerable<T> Paginate<T>(PaginationQuery paginationQuery, IEnumerable<T> collection)
        {
            if(paginationQuery.StartFrom.HasValue)
            {
                collection = collection.Skip(paginationQuery.StartFrom.Value);
            }
            if(paginationQuery.Quantity.HasValue)
            {
                collection = collection.Take(paginationQuery.Quantity.Value);
            }
            return collection;
        }
    }
}
