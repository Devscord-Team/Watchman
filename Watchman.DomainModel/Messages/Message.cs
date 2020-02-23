using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Messages
{
    public class Message : Entity, IAggregateRoot
    {
        public User Author { get; private set; }
        public Channel Channel { get; private set; }
        public Server Server { get; private set; }
        public string Content { get; private set; }

        private Message(string content)
        {
            this.Content = content;
        }

        public static MessageBuilder Create(string content)
        {
            return new MessageBuilder(new Message(content));
        }

        public void SetAuthor(User author)
        {
            if(author == this.Author)
            {
                return;
            }
            this.Author = author;
            this.Update();
        }

        public void SetChannel(Channel channel)
        {
            if (channel == this.Channel)
            {
                return;
            }
            this.Channel = channel;
            this.Update();
        }

        public void SetServer(Server server)
        {
            if (server == this.Server)
            {
                return;
            }
            this.Server = server;
            this.Update();
        }

        public void SetContent(string content)
        {
            if (content == this.Content)
            {
                return;
            }
            this.Content = content;
            this.Update();
        }
    }

}
