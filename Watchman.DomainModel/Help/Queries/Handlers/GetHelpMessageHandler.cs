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
            var allText = File.ReadAllText(_helpFileName);
            var helpInfos = new List<HelpInformation>();

            // todo: parse json file to list

            return new GetHelpInformationQueryResult(helpInfos);
        }
    }
}
