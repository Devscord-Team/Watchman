namespace Watchman.Web.Areas.Messages.Models.Dtos
{
    public class MessageChannelDto
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public MessageChannelDto(ulong id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
