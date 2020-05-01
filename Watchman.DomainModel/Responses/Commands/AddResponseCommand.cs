using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Commands
{
    public class AddResponseCommand : ICommand
    {
        public Response Response { get; }

        public AddResponseCommand(Response response)
        {
            Response = response;
        }
    }
}