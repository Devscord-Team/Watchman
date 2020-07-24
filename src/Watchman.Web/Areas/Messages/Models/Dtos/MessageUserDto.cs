namespace Watchman.Web.Areas.Messages.Models.Dtos
{
    public class MessageUserDto
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public MessageUserDto(ulong id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
