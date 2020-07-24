namespace Watchman.DomainModel.Messages
{
    public class Channel
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public Channel(ulong id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }

}
