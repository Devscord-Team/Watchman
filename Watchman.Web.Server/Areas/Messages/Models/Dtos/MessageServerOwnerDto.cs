using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Server.Areas.Messages.Models.Dtos
{
    public class MessageServerOwnerDto
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public MessageServerOwnerDto(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
