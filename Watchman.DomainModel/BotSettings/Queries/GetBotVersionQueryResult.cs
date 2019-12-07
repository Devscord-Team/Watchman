using System;
using Watchman.Cqrs;

namespace Watchman.DomainModel.BotSettings.Queries
{
    public class GetBotVersionQueryResult : IQueryResult
    {
        public string Version { get; }

        public GetBotVersionQueryResult(string version)
        {
            Version = version;
        }
    }
}