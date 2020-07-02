using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.CustomCommands.Commands.Handlers
{
    public class DeleteCustomCommandsCommandHandler : ICommandHandler<DeleteCustomCommandsCommand>
    {
        private readonly ISessionFactory sessionFactory;

        public DeleteCustomCommandsCommandHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(DeleteCustomCommandsCommand command)
        {
            using var session = this.sessionFactory.Create();
            var customCommand = session.Get<CustomCommand>().FirstOrDefault(x => x.ServerId == command.ServerId && x.CommandFullName == command.CommandFullName);
            if (customCommand != null)
            {
                await session.DeleteAsync(customCommand);
            }
        }
    }
}
