using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.Commons.Queries;

namespace Watchman.DomainModel.Messages.Queries
{
    public class GetMessagesQuery : PaginationQuery, IQuery<GetMessagesQueryResult>
    {
    }
}
