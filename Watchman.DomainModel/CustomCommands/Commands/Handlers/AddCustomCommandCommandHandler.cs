using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.CustomCommands.Commands.Handlers
{
    public class AddCustomCommandCommandHandler : ICommandHandler<AddCustomCommandsCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddCustomCommandCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddCustomCommandsCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            var templateRegex = command.CustomTemplateRegex.StartsWith('^')
                ? command.CustomTemplateRegex
                : $"^{command.CustomTemplateRegex}";
            var customCommand = new CustomCommand(command.CommandFullName, templateRegex, command.ServerId);
            await session.AddAsync(customCommand);
        }
    }
}
