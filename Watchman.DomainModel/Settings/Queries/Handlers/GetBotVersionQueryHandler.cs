using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Settings.Queries.Handlers
{
    public class GetBotVersionQueryHandler : IQueryHandler<GetBotVersionQuery, GetBotVersionQueryResult>
    {
        public GetBotVersionQueryResult Handle(GetBotVersionQuery query)
        {
            const string fileName = "version.txt";

            var version = File.ReadAllText(fileName);
            return new GetBotVersionQueryResult(version);
        }
    }
}
