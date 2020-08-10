namespace Watchman.Web.Areas.Messages.Models.Dtos
{
    public class MessageServerDto
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public MessageServerDto(ulong id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
