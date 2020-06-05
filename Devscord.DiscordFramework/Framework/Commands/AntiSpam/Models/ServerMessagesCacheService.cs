using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models
{
    public class ServerMessagesCacheService
    {
        private static Dictionary<ulong, List<SmallMessage>> _usersMessages;

        static ServerMessagesCacheService()
        {
            _usersMessages = new Dictionary<ulong, List<SmallMessage>>();
            RemoveOldMessagesCyclic();
        }

        public ServerMessagesCacheService()
        {
        }

        public void OverwriteMessages(IEnumerable<SmallMessage> smallMessages)
        {
            _usersMessages = smallMessages.GroupBy(x => x.UserId).ToDictionary(x => x.Key, x => x.ToList());
        }

        public void AddMessage(SmallMessage smallMessage)
        {
            _usersMessages[smallMessage.UserId].Add(smallMessage);
        }

        public void AddMessage(DiscordRequest request, Contexts contexts)
        {
            var smallMessage = new SmallMessage(request.OriginalMessage, contexts.User.Id, request.SentAt);
            this.AddMessage(smallMessage);
        }

        public List<SmallMessage> GetLastUserMessages(ulong userId)
        {
            return _usersMessages[userId];
        }

        private static async void RemoveOldMessagesCyclic()
        {
            await Task.Delay(TimeSpan.FromMinutes(2));
            var minTimeInPast = DateTime.Now.AddMinutes(-5);
            _usersMessages = _usersMessages.Values.Select(list =>
            {
                list.RemoveAll(message => message.SentAt < minTimeInPast);
                return list;
            }).ToDictionary(x => x.First().UserId, x => x);
        }
    }
}
