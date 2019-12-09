using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Queries.Handlers
{
    public class GetHelpMessageQueryHandler : IQueryHandler<GetHelpMessageQuery, GetHelpMessageQueryResult>
    {
        public GetHelpMessageQueryResult Handle(GetHelpMessageQuery query)
        {

            var messageBuilder = new StringBuilder();
            throw new NotImplementedException();
            return new GetHelpMessageQueryResult(messageBuilder.ToString());
        }
    }
}
