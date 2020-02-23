using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.DomainModel.Commons.Queries
{
    public abstract class PaginationQuery
    {
        public int? StartFrom { get; set; }
        public int? Quantity { get; set; }
    }

    public abstract class PaginationQuery<T> : PaginationQuery
    {
        public Func<T, bool> Filter { get; set; }
    }
}
