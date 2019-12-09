using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Help.Queries
{
    public class GetHelpMessageQuery : IQuery<GetHelpMessageQueryResult>
    {
        public ISession Session { get; }

        public GetHelpMessageQuery(ISession session)
        {
            this.Session = session;
        }
    }
}
