namespace Watchman.DomainModel.Messages
{
    public class User
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public User(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }

}
