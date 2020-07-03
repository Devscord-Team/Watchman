using System;
using Watchman.Common.Models;

namespace Watchman.DomainModel.Commons.Queries
{
    public abstract class PaginationQuery
    {
        public int? Skip { get; set; }
        public int? Quantity { get; set; }
        public TimeRange CreatedDate { get; set; }
        public TimeRange UpdatedDate { get; set; }
    }

    public abstract class PaginationQuery<T> : PaginationQuery
    {
        public Func<T, bool> Filter { get; set; }
    }
}
