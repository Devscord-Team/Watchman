using System;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord.Areas.Statistics.Models
{
    public class MessageInformation : Entity
    {
        public MessageInformationAuthor Author { get; set; }
        public MessageInformationChannel Channel { get; set; }
        public MessageInformationServer Server { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
    }
}
