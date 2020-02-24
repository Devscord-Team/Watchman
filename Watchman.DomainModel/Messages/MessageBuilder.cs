namespace Watchman.DomainModel.Messages
{
    public class MessageBuilder
    {
        private readonly Message message;

        internal MessageBuilder(Message message)
        {
            this.message = message;
        }

        public MessageBuilder WithAuthor(ulong id, string name)
        {
            var author = new User(id, name);
            message.SetAuthor(author);
            return this;
        }

        public MessageBuilder WithChannel(ulong id, string name)
        {
            var channel = new Channel(id, name);
            message.SetChannel(channel);
            return this;
        }

        public MessageBuilder WithServer(ulong id, string name, ulong ownerId, string ownerName)
        {
            var owner = new User(ownerId, ownerName);
            var server = new Server(id, name, owner);
            message.SetServer(server);
            return this;
        }

        public Message Build() => this.message;
    }

}
