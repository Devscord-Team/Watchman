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
        private HashSet<string> _cashedExistingMessagesHashes;
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
            var newMessages = this.GetOnlyNewMessages(command.Messages, command.ChannelId);
            using var session = this._sessionFactory.Create();
            foreach (var message in newMessages)
            {
                await session.AddAsync(message);
            }
        }

        private IEnumerable<Message> GetOnlyNewMessages(IEnumerable<Message> messages, ulong channelId)
        {
            var existingHashes = this.GetExistingHashes(channelId);
            return messages.Where(x => !existingHashes.Contains(this._md5HashService.GetHash(x)));
        }

        private HashSet<string> GetExistingHashes(ulong channelId)
        {
            if (this._cashedChannelId == channelId)
            {
                return this._cashedExistingMessagesHashes;
            }
            using var session = this._sessionFactory.Create();
            var channelMessagesHashes = session.Get<Message>()
                .Where(x => x.Channel.Id == channelId)
                .Select(x => x.Md5Hash)
                .ToHashSet();

            this._cashedChannelId = channelId;
            this._cashedExistingMessagesHashes = channelMessagesHashes;
            return channelMessagesHashes;
        }
    }
}
