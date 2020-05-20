using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.CustomCommands.Commands.Handlers
{
    public class AddCustomCommandsCommandHandler : ICommandHandler<AddCustomCommandsCommand>
    {
        private readonly ISessionFactory sessionFactory;

        public AddCustomCommandsCommandHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddCustomCommandsCommand command)
        {
            using var session = this.sessionFactory.Create();
            var customCommand = new CustomCommand(command.CommandFullName, command.CustomTemplateRegex, command.ServerId);
            await session.AddAsync(customCommand);
        }
    }
}
