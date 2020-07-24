namespace Watchman.Web.Areas.Messages.Models.Dtos
{
    public class MessageServerDto
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public MessageServerOwnerDto Owner { get; private set; }

        public MessageServerDto(ulong id, string name, MessageServerOwnerDto owner)
        {
            this.Id = id;
            this.Name = name;
            this.Owner = owner;
        }
    }
}
