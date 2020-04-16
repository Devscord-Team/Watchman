using Watchman.Common.Models;
using Watchman.DomainModel.Commons.Queries;

namespace Watchman.DomainModel.Messages.Queries
{
    public class PaginationMessagesQuery : PaginationQuery
    {
        public TimeRange SentDate { get; set; }
    }
}
