using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Commands.Handlers
{
    public class AddMessageCommandHandler : ICommandHandler<AddMessageCommand>
    {
        private readonly ISessionFactory sessionFactory;

        public AddMessageCommandHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public Task HandleAsync(AddMessageCommand command)
        {
            var message = Message.Create(command.Content)
                .WithAuthor(command.AuthorId, command.AuthorName)
                .WithChannel(command.ChannelId, command.ChannelName)
                .WithServer(command.ServerId, command.ServerName, command.ServerOwnerId, command.ServerOwnerName)
                .Build();

            using(var session = this.sessionFactory.Create())
            {
                session.Add(message);
            }
            return Task.CompletedTask;
        }
    }
}
