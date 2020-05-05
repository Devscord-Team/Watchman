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
        public MessageUserDto User { get; private set; }
        public MessageChannelDto Channel { get; private set; }
        public MessageServerDto Server { get; private set; }
        public DateTime SentAt { get; private set; }

        public MessageDto()
        {
        }

        public MessageDto(Message message)
        {
            Content = message.Content;
            User = new MessageUserDto(message.Author.Id, message.Author.Name);
            Channel = new MessageChannelDto(message.Channel.Id, message.Channel.Name);
            Server = new MessageServerDto(message.Server.Id, message.Server.Name, message.Server.Owner.Id, message.Server.Owner.Name);
            SentAt = message.SentAt;
        }
    }
    public class MessageUserDto
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public MessageUserDto(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class MessageServerDto
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public ulong OwnerId { get; private set; }
        public string OwnerName { get; private set; }

        public MessageServerDto(ulong id, string name, ulong ownerId, string ownerName)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            OwnerName = ownerName;
        }
    }

    public class MessageChannelDto
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public MessageChannelDto(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
