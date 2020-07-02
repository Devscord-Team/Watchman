using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages.Services;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages.Commands.Handlers
{
    public class AddMessagesCommandHandler : ICommandHandler<AddMessagesCommand>
    {
        private List<string> _cashedExistingMessagesHashes;
        private ulong _cashedChannelId;

        private readonly ISessionFactory _sessionFactory;
        private readonly Md5HashService _md5HashService;

        public AddMessagesCommandHandler(ISessionFactory sessionFactory, Md5HashService md5HashService)
        {
            this._sessionFactory = sessionFactory;
            this._md5HashService = md5HashService;
        }

        public async Task HandleAsync(AddMessagesCommand command)
        {
            var newMessages = this.GetOnlyNewMessages(command.Messages, command.ChannelId).ToList();
            using var session = this._sessionFactory.Create();
            foreach (var message in newMessages)
            {
                await session.AddAsync(message);
            }
        }

        private IEnumerable<Message> GetOnlyNewMessages(IEnumerable<Message> messages, ulong channelId)
        {
            using var session = this._sessionFactory.Create();
            var existingHashes = this.GetExistingHashes(channelId);

            return messages.Where(x =>
            {
                var isMessageNew = existingHashes.BinarySearch(this._md5HashService.GetHash(x)) < 0;
                return isMessageNew;
            });
        }

        private List<string> GetExistingHashes(ulong channelId)
        {
            if (this._cashedChannelId == channelId)
            {
                return this._cashedExistingMessagesHashes;
            }

            using var session = this._sessionFactory.Create();
            var channelMessagesHashes = session.Get<Message>()
                .Where(x => x.Channel.Id == channelId)
                .Select(x => x.Md5Hash)
                .ToList() // ToList must be here
                .OrderBy(x => x)
                .ToList();

            this._cashedChannelId = channelId;
            this._cashedExistingMessagesHashes = channelMessagesHashes;
            return channelMessagesHashes;
        }
    }
}
