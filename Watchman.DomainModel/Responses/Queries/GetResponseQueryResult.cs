using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Queries
{
    public class GetResponseQueryResult : IQueryResult
    {
        public Response Response { get; private set; }

        public GetResponseQueryResult(Response response)
        {
            Response = response;
        }
    }
}
