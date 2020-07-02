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

        public void OverwriteMessages(IEnumerable<SmallMessage> smallMessages)
        {
            _usersMessages = smallMessages.GroupBy(x => x.UserId).ToDictionary(x => x.Key, x => x.ToList());
        }

        public void AddMessage(SmallMessage smallMessage)
        {
            if (_usersMessages.ContainsKey(smallMessage.UserId))
            {
                _usersMessages[smallMessage.UserId].Add(smallMessage);
            }
            else
            {
                _usersMessages.Add(smallMessage.UserId, new List<SmallMessage> { smallMessage });
            }
        }

        public void AddMessage(DiscordRequest request, Contexts contexts)
        {
            var smallMessage = new SmallMessage(request.OriginalMessage, contexts.User.Id, request.SentAt, contexts.Server.Id);
            this.AddMessage(smallMessage);
        }

        public IEnumerable<SmallMessage> GetLastUserMessages(ulong userId, ulong serverId)
        {
            return _usersMessages.TryGetValue(userId, out var smallMessages)
                ? smallMessages.Where(x => x.ServerId == serverId)
                : new List<SmallMessage>();
        }

        private static async void RemoveOldMessagesCyclic()
        {
            await Task.Delay(TimeSpan.FromMinutes(2));
            var minTimeInPast = DateTime.UtcNow.AddMinutes(-5);
            var smallMessages = _usersMessages.Values.Select(list =>
            {
                list.RemoveAll(message => message.SentAt < minTimeInPast);
                return list;
            }).ToList();
            if (smallMessages.Count == 0)
            {
                return;
            }
            _usersMessages = smallMessages.ToDictionary(x => x.First().UserId, x => x);
        }
    }
}
