using System;
using TypeGen.Core.TypeAnnotations;
using Watchman.DomainModel.Messages;

namespace Watchman.Web.Areas.Messages.Models.Dtos
{
    [ExportTsClass(OutputDir = "ClientApp/src/models")]
    public class MessageDto
    {
        public string Content { get; set; }
        public MessageUserDto User { get; set; }
        public MessageChannelDto Channel { get; set; }
        public MessageServerDto Server { get; set; }
        public DateTime SentAt { get; set; }

        public MessageDto()
        {
        }

        public MessageDto(Message message)
        {
            this.Content = message.Content;
            this.User = new MessageUserDto(message.Author.Id, message.Author.Name);
            this.Channel = new MessageChannelDto(message.Channel.Id, message.Channel.Name);
            this.Server = new MessageServerDto(message.Server.Id, message.Server.Name);
            this.SentAt = message.SentAt;
        }
    }
}
