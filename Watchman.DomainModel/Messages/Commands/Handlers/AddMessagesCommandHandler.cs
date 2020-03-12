using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Commands.Handlers
{
    public class AddMessagesCommandHandler : ICommandHandler<AddMessagesCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public AddMessagesCommandHandler(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddMessagesCommand command)
        {
            var newMessages = await Task.Run(() => GetOnlyNewMessages(command.Messages));
            using var session = _sessionFactory.Create();
            session.Add(newMessages);
        }

        private IEnumerable<Message> GetOnlyNewMessages(IEnumerable<Message> messages)
        {
            using var session = _sessionFactory.Create();
            var existingMessagesIds = session.Get<Message>()
                .Select(x => x.Id.GetHashCode())
                .OrderBy(x => x)
                .ToList();

            return messages.Where(x =>
            {
                var isMessageNew = existingMessagesIds.BinarySearch(x.Id.GetHashCode()) == -1;
                return isMessageNew;
            });
        }
    }
}
