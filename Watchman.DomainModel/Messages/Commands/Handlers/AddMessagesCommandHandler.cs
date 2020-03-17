using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            foreach (var message in newMessages)
            {
                session.Add(message);
            };
        }

        private IEnumerable<Message> GetOnlyNewMessages(IEnumerable<Message> messages)
        {
            using var session = _sessionFactory.Create();
            var allMessages = session.Get<Message>().ToList(); // ToList must be here
            var allIds = allMessages.Select(GetHash);
            var existingMessagesIds = allIds
                .OrderBy(x => x)
                .ToList();

            return messages.Where(x =>
            {
                var isMessageNew = existingMessagesIds.BinarySearch(GetHash(x)) < 0;
                return isMessageNew;
            });
        }

        private string GetHash(Message message)
        {
            var stringToMd5 = message.SentAt.Ticks.ToString() + message.Author;
            var bytes = Encoding.ASCII.GetBytes(stringToMd5);
            using var md5 = MD5.Create();
            var hashed = md5.ComputeHash(bytes);

            var builder = new StringBuilder();
            foreach (var b in hashed)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
