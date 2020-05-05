using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Server.Areas.Messages.Models.Dtos
{
    public class MessageServerDto
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public MessageServerOwnerDto Owner { get; private set; }

        public MessageServerDto(ulong id, string name, MessageServerOwnerDto owner)
        {
            Id = id;
            Name = name;
            Owner = owner;
        }
    }
}
