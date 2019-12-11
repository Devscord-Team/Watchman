using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help.Queries
{
    public class GetHelpInformationQuery : IQuery<GetHelpInformationQueryResult>
    {
        public ISession Session { get; }

        public GetHelpInformationQuery(ISession session)
        {
            this.Session = session;
        }
    }
}
