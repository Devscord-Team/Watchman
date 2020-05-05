using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.DomainModel.Messages;

namespace Watchman.Web.Server.Areas.Messages.Models.Dtos
{
    public class MessageDto
    {
        public string Content { get; private set; }
        public ulong AuthorId { get; private set; }
        public string AuthorName { get; private set; }
        public ulong ChannelId { get; private set; }
        public string ChannelName { get; private set; }
        public ulong ServerId { get; private set; }
        public string ServerName { get; private set; }
        public ulong ServerOwnerId { get; private set; }
        public string ServerOwnerName { get; private set; }
        public DateTime SentAt { get; private set; }

        public MessageDto()
        {
        }

        public MessageDto(Message message)
        {
            Content = message.Content;
            AuthorId = message.Author.Id;
            AuthorName = message.Author.Name;
            ChannelId = message.Channel.Id;
            ChannelName = message.Channel.Name;
            ServerId = message.Server.Id;
            ServerName = message.Server.Name;
            ServerOwnerId = message.Server.Owner.Id;
            ServerOwnerName = message.Server.Owner.Name;
            SentAt = message.SentAt;
        }
    }
}
