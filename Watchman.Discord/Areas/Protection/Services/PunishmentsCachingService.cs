using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Users;
using Watchman.DomainModel.Users.Commands;
using Watchman.DomainModel.Users.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class PunishmentsCachingService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private static Dictionary<ulong, List<Punishment>> _punishments;

        public PunishmentsCachingService(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _punishments = LoadUsersPunishmentsFromDomain();
        }

        public List<Punishment> GetUserPunishments(ulong userId)
        {
            return _punishments[userId];
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
            
            var option = (ProtectionPunishmentOption)(int)punishment.PunishmentOption;
            var protectionPunishment = new ProtectionPunishment(option, userId, punishment.GivenAt, punishment.ForTime);
            var command = new AddProtectionPunishmentCommand(protectionPunishment);
            await _commandBus.ExecuteAsync(command);
        }

        private Dictionary<ulong, List<Punishment>> LoadUsersPunishmentsFromDomain()
        {
            var query = new GetProtectionPunishmentsQuery();
            var protectionPunishments = _queryBus.Execute(query).ProtectionPunishments;
            return protectionPunishments
                .GroupBy(x => x.UserId)
                .Select(x =>
                {
                    var userId = x.Key;
                    var punishments = x.Select(x =>
                    {
                        var option = (PunishmentOption)(int)x.Option;
                        return new Punishment(option, x.GivenAt, x.Time);
                    }).ToList();
                    return new KeyValuePair<ulong, List<Punishment>>(userId, punishments);
                })
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
