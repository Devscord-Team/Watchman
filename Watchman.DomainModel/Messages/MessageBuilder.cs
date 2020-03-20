using System;

namespace Watchman.DomainModel.Messages
{
    public class MessageBuilder
    {
        private readonly Message _message;

        internal MessageBuilder(Message message)
        {
            this._message = message;
        }

        public MessageBuilder WithAuthor(ulong id, string name)
        {
            var author = new User(id, name);
            _message.SetAuthor(author);
            return this;
        }

        public MessageBuilder WithChannel(ulong id, string name)
        {
            var channel = new Channel(id, name);
            _message.SetChannel(channel);
            return this;
        }

        public MessageBuilder WithServer(ulong id, string name, ulong ownerId, string ownerName)
        {
            var owner = new User(ownerId, ownerName);
            var server = new Server(id, name, owner);
            _message.SetServer(server);
            return this;
        }

        public MessageBuilder WithSentAtDate(DateTime sentAt)
        {
            _message.SetSentAt(sentAt);
            return this;
        }

        public Message Build()
        {
            this._message.SetHash();
            return this._message;
        }
    }
}
