using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Common;
using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Users;
using Watchman.DomainModel.Users.Commands;
using Watchman.DomainModel.Users.Queries;
using Watchman.DomainModel.Warns;
using Watchman.DomainModel.Warns.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class PunishmentsCachingService : IPunishmentsCachingService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private static Dictionary<ulong, List<Punishment>> _punishments;
        private static FriendlyDictionary<ulong, FriendlyDictionary<ulong, List<WarnEvent>>> _warnsByServer;

        static PunishmentsCachingService()
        {
            _warnsByServer = new FriendlyDictionary<ulong, FriendlyDictionary<ulong, List<WarnEvent>>>();
        }

        public PunishmentsCachingService(IQueryBus queryBus, ICommandBus commandBus)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            _punishments = this.LoadUsersPunishmentsFromDomain();
        }

        public List<Punishment> GetUserPunishments(ulong userId)
        {
            return _punishments[userId];
        }

        public int GetUserMutesCount(ulong userId, DateTime? since = null)
        {
            var wasFound = _punishments.TryGetValue(userId, out var punishments);
            if (!wasFound)
            {
                return 0;
            }
            var mutes = punishments.Where(x => x.PunishmentOption == PunishmentOption.Mute);
            return since.HasValue
                ? mutes.Count(x => x.GivenAt >= since)
                : mutes.Count();
        }

        public async Task AddUserPunishment(ulong userId, Punishment punishment)
        {
            if (_punishments.ContainsKey(userId))
            {
                _punishments[userId].Add(punishment);
            }
            else
            {
                _punishments.Add(userId, new List<Punishment> { punishment });
            }

            var option = (ProtectionPunishmentOption) (int) punishment.PunishmentOption;
            var protectionPunishment = new ProtectionPunishment(option, userId, punishment.GivenAt, punishment.ForTime);
            var command = new AddProtectionPunishmentCommand(protectionPunishment);
            await this._commandBus.ExecuteAsync(command);
        }

        public async Task ClearAndLoadWarnEventsToCache()
        {
            _warnsByServer = new FriendlyDictionary<ulong, FriendlyDictionary<ulong, List<WarnEvent>>>();
            var query = new GetWarnEventsQuery(serverId: 0, receiverId: 0, new TimeRange(new DateTime(), DateTime.Now));
            var warns = (await this._queryBus.ExecuteAsync(query)).WarnEvents;
            foreach (var warn in warns)
            {
                this.AddWarnToDictionary(warn);
            }
        }

        public int GetWarnsCount(ulong serverId, ulong userId, DateTime since)
        {
            return _warnsByServer[serverId][userId].Count;
        }

        public void AddWarnLocal(ulong grantorId, ulong receiverId, string reason, ulong serverId)
        {
            var warnEvent = new WarnEvent(grantorId, receiverId, reason, serverId);
            this.AddWarnToDictionary(warnEvent);
        }

        public void RemoveWarnsLocal(ulong serverId, ulong userId, DateTime from)
        {
            _warnsByServer[serverId][userId].RemoveAll(x => x.CreatedAt >= from);
        }

        private Dictionary<ulong, List<Punishment>> LoadUsersPunishmentsFromDomain()
        {
            var query = new GetProtectionPunishmentsQuery();
            var protectionPunishments = this._queryBus.Execute(query).ProtectionPunishments;
            return protectionPunishments
                .GroupBy(x => x.UserId)
                .Select(x =>
                {
                    var userId = x.Key;
                    var punishments = x.Select(x =>
                    {
                        var option = (PunishmentOption)(int)x.Option;
                        return new Punishment(option, x.GivenAt, x.Time);
                    });
                    return new KeyValuePair<ulong, IEnumerable<Punishment>>(userId, punishments);
                })
                .ToDictionary(x => x.Key, x => x.Value.ToList());
        }

        private void AddWarnToDictionary(WarnEvent warnToAdd)
        {
            _warnsByServer[warnToAdd.ServerId][warnToAdd.ReceiverId].Add(warnToAdd);
        }
    }
}
