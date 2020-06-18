using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Queries
{
    public class GetResponsesQueryResult : IQueryResult
    {
        public IEnumerable<Response> Responses { get; }

        public GetResponsesQueryResult(IEnumerable<Response> responses) => this.Responses = responses;
    }
}