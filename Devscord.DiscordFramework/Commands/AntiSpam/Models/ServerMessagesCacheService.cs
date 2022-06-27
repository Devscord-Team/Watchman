using Devscord.DiscordFramework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Commands.AntiSpam.Models
{
    public interface IServerMessagesCacheService
    {
        void OverwriteMessages(IEnumerable<SmallMessage> smallMessages);
        void AddMessage(SmallMessage smallMessage);
        void AddMessage(DiscordRequest request, Contexts contexts);
        IEnumerable<SmallMessage> GetLastUserMessages(ulong userId, ulong serverId);
    }

    public class ServerMessagesCacheService : IServerMessagesCacheService
    {
        private static Dictionary<ulong, List<SmallMessage>> _usersMessages = new();

        public void OverwriteMessages(IEnumerable<SmallMessage> smallMessages)
        {
            _usersMessages = smallMessages.GroupBy(x => x.UserId).ToDictionary(x => x.Key, x => x.ToList());
        }

        public void AddMessage(SmallMessage smallMessage)
        {
            if (_usersMessages.ContainsKey(smallMessage.UserId))
            {
                _usersMessages[smallMessage.UserId].Add(smallMessage);
                return;
            }
            _usersMessages.Add(smallMessage.UserId, new List<SmallMessage> { smallMessage });
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

        public static void RemoveOldMessagesCyclic()
        {
            var minTimeInPast = DateTime.UtcNow.AddMinutes(-15);
            var smallMessages = _usersMessages.Values.Select(list =>
            {
                list.RemoveAll(message => message.SentAt < minTimeInPast);
                return list;
            })
            .Where(x => x.Any())
            .ToList();

            if (!smallMessages.Any())
            {
                return;
            }
            _usersMessages = smallMessages.ToDictionary(x => x.First().UserId, x => x);
        }
    }
}
