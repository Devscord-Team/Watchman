using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Commands.Handlers
{
    public class AddMessageCommandHandler : ICommandHandler<AddMessageCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddMessageCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public Task HandleAsync(AddMessageCommand command)
        {
            var message = Message.Create(command.Content)
                .WithAuthor(command.AuthorId, command.AuthorName)
                .WithChannel(command.ChannelId, command.ChannelName)
                .WithServer(command.ServerId, command.ServerName, command.ServerOwnerId, command.ServerOwnerName)
                .WithSentAtDate(command.SentAt)
                .Build();

            using(var session = this._sessionFactory.Create())
            {
                session.Add(message);
            }
            return Task.CompletedTask;
        }
    }
}
