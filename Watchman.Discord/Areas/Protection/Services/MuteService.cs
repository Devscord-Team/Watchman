using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Common.Exceptions;
using Watchman.Common.Models;
using Watchman.DomainModel.Mute;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class MuteService
    {
        public UserRole MuteRole { get; private set; }
        public UserContext MutedUser { get; private set; }
        public MuteEvent MuteEvent { get; private set; }

        private readonly UsersService _usersService;
        private readonly UsersRolesService _usersRolesService;
        private readonly Contexts _contexts;
        private readonly DiscordRequest _request;

        public MuteService(UsersService usersService, UsersRolesService usersRolesService, Contexts contexts, DiscordRequest request)
        {
            this._usersService = usersService;
            this._usersRolesService = usersRolesService;
            this._contexts = contexts;
            this._request = request;
        }

        public async Task MuteUser()
        {
            var mention = GetMention();
            MutedUser = FindUserByMention(mention);
            MuteRole = GetMuteRole();
            MuteEvent = CreateMuteEvent(MutedUser.Id);

            await AssignMuteRoleAsync(MuteRole, MutedUser);
        }

        public async Task UnmuteUser()
        {
            await RemoveMuteRoleAsync(MuteRole, MutedUser);
        }

        private string GetMention()
        {
            var mention = _request.Arguments.FirstOrDefault()?.Values.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(mention))
            {
                throw new UserDidntMentionedAnyUserToMuteException();
            }
            return mention;
        }

        private UserContext FindUserByMention(string mention)
        {
            var userToMute = _usersService.GetUsers(_contexts.Server)
                .FirstOrDefault(x => x.Mention == mention);

            if (userToMute == null)
            {
                throw new UserNotFoundException(mention);
            }
            return userToMute;
        }

        private UserRole GetMuteRole()
        {
            var muteRole = _usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, _contexts.Server);

            if (muteRole == null)
            {
                throw new RoleNotFoundException(UsersRolesService.MUTED_ROLE_NAME);
            }
            return muteRole;
        }

        private MuteEvent CreateMuteEvent(ulong userId)
        {
            var reason = _request.Arguments.FirstOrDefault(x => x.Name == "reason")?.Values.FirstOrDefault();
            var forTime = _request.Arguments.FirstOrDefault(x => x.Name == "time")?.Values.FirstOrDefault();

            var timeRange = new TimeRange()
            {
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow + ParseToTimeSpan(forTime)
            };

            return new MuteEvent(userId, timeRange, reason, _contexts.Server.Id);
        }

        private static TimeSpan ParseToTimeSpan(string time)
        {
            var defaultTime = TimeSpan.FromHours(1);

            if (string.IsNullOrWhiteSpace(time))
                return defaultTime;

            var lastChar = time[^1];
            time = time[..^1];
            time = time.Replace(',', '.');
            double.TryParse(time, NumberStyles.Any, CultureInfo.InvariantCulture, out var asNumber);

            return lastChar switch
            {
                'm' => TimeSpan.FromMinutes(asNumber),
                'h' => TimeSpan.FromHours(asNumber),
                _ => defaultTime,
            };
        }

        private async Task AssignMuteRoleAsync(UserRole muteRole, UserContext userToMute)
        {
            await _usersService.AddRole(muteRole, userToMute, _contexts.Server);
        }

        private async Task RemoveMuteRoleAsync(UserRole muteRole, UserContext userToUnmute)
        {
            await _usersService.RemoveRole(muteRole, userToUnmute, _contexts.Server);
        }
    }
}
