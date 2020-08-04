namespace Watchman.DomainModel.Messages
{
    public class Server
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public Server(ulong id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
