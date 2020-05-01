using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Queries
{
    public class GetResponseQuery : IQuery<GetResponseQueryResult>
    {
        public string OnEvent { get; private set; }
        public ulong ServerId { get; private set; }

        public GetResponseQuery(string onEvent, ulong serverId = 0)
        {
            OnEvent = onEvent;
            ServerId = serverId;
        }
    }
}
