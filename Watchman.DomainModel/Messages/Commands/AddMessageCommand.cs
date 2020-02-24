using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Messages.Commands
{
    public class AddMessageCommand : ICommand
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

        public AddMessageCommand(string content, 
            ulong authorId, string authorName, 
            ulong channelId, string channelName,
            ulong serverId, string serverName,
            ulong serverOwnerId, string serverOwnerName)
        {
            Content = content;
            AuthorId = authorId;
            AuthorName = authorName;
            ChannelId = channelId;
            ChannelName = channelName;
            ServerId = serverId;
            ServerName = serverName;
            ServerOwnerId = serverOwnerId;
            ServerOwnerName = serverOwnerName;
        }
    }
}
