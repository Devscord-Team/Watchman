namespace Watchman.Discord.Areas.Statistics.Models
{
    public class MessageInformationServer
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public MessageInformationAuthor Owner { get; set; }
    }
}
