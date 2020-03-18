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
        private List<string> _cashedExistingMessagesHashes;
        private ulong _cashedChannelId;

        private readonly ISessionFactory _sessionFactory;

        public AddMessagesCommandHandler(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(AddMessagesCommand command)
        {
            var newMessages = await Task.Run(() => GetOnlyNewMessages(command.Messages, command.ChannelId));
            using var session = _sessionFactory.Create();
            foreach (var message in newMessages)
            {
                session.Add(message);
            };
        }

        private IEnumerable<Message> GetOnlyNewMessages(IEnumerable<Message> messages, ulong channelId)
        {
            using var session = _sessionFactory.Create();
            var existingHashes = GetExistingHashes(channelId);

            return messages.Where(x =>
            {
                var isMessageNew = existingHashes.BinarySearch(GetHash(x)) < 0;
                return isMessageNew;
            });
        }

        private List<string> GetExistingHashes(ulong channelId)
        {
            if (_cashedChannelId == channelId)
            {
                return _cashedExistingMessagesHashes;
            }

            using var session = _sessionFactory.Create();
            var channelMessagesHashes = session.Get<Message>()
                .Where(x => x.Channel.Id == channelId)
                .AsEnumerable()
                .Select(GetHash)
                .ToList() // ToList must be here
                .OrderBy(x => x)
                .ToList(); 

            _cashedChannelId = channelId;
            _cashedExistingMessagesHashes = channelMessagesHashes;
            return channelMessagesHashes;
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
