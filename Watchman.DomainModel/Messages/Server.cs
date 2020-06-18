namespace Watchman.DomainModel.Messages
{
    public class Server
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public User Owner { get; private set; }

        public Server(ulong id, string name, User owner)
        {
            this.Id = id;
            this.Name = name;
            this.Owner = owner;
        }
    }

}
