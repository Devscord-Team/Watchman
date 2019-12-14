using Watchman.Cqrs;

namespace Watchman.DomainModel.Help.Queries
{
    public class GetHelpMessageQueryResult : IQueryResult
    {
        public string HelpMessage { get; }

        public GetHelpMessageQueryResult(string helpMessage)
        {
            this.HelpMessage = helpMessage;
        }
    }
}
