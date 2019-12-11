using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Queries.Handlers
{
    public class GetHelpInformationsQueryHandler : IQueryHandler<GetHelpInformationQuery, GetHelpInformationQueryResult>
    {
        private const string _helpFileName = "helpInformation.json";

        public GetHelpInformationQueryResult Handle(GetHelpInformationQuery query)
        {
            using var helpFile = new StreamReader(_helpFileName);
            var helpInfos = new List<HelpInformation>();
            
            throw new NotImplementedException();
            return new GetHelpInformationQueryResult(helpInfos);
        }
    }
}
