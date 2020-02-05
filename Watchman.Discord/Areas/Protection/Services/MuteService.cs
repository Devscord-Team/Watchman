using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
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

        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UsersService _usersService;
        private readonly UsersRolesService _usersRolesService;
        private readonly Contexts _contexts;

        public MuteService(MessagesServiceFactory messagesServiceFactory, UsersService usersService, UsersRolesService usersRolesService, Contexts contexts)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._usersService = usersService;
            this._usersRolesService = usersRolesService;
            this._contexts = contexts;
        }

        public async Task MuteUser(DiscordRequest request)
        {
            var mention = GetMention(request);
            MutedUser = FindUserByMention(mention, _contexts.Server);
            MuteRole = GetMuteRole(_contexts);
            MuteEvent = CreateMuteEvent(MutedUser.Id, request);

            await AssignMuteRoleAsync(MuteRole, MutedUser, _contexts.Server);
        }

        public void UnmuteUser()
        {
            RemoveMuteRoleAsync(MuteRole, MutedUser, _contexts.Server);
        }

        private string GetMention(DiscordRequest request)
        {
            var mention = request.Arguments.FirstOrDefault()?.Values.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(mention))
            {
                throw new UserDidntMentionedAnyUserToMuteException();
            }
            return mention;
        }

        private UserContext FindUserByMention(string mention, DiscordServerContext server)
        {
            var userToMute = _usersService.GetUsers(server).FirstOrDefault(x => x.Mention == mention);

            if (userToMute == null)
            {
                throw new UserNotFoundException(mention);
            }
            return userToMute;
        }

        private UserRole GetMuteRole(Contexts contexts)
        {
            var muteRole = _usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, contexts.Server);

            if (muteRole == null)
            {
                throw new RoleNotFoundException(UsersRolesService.MUTED_ROLE_NAME);
            }
            return muteRole;
        }

        private MuteEvent CreateMuteEvent(ulong userId, DiscordRequest request)
        {
            var reason = request.Arguments.FirstOrDefault(x => x.Name == "reason")?.Values.FirstOrDefault();
            var forTime = request.Arguments.FirstOrDefault(x => x.Name == "time")?.Values.FirstOrDefault();

            var timeRange = new TimeRange()
            {
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow + ParseToTimeSpan(forTime)
            };

            return new MuteEvent(userId, timeRange, reason);
        }

        private TimeSpan ParseToTimeSpan(string time)
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

        private async Task AssignMuteRoleAsync(UserRole muteRole, UserContext userToMute, DiscordServerContext serverContext)
        {
            await _usersService.AddRole(muteRole, userToMute, serverContext);
        }

        private async Task RemoveMuteRoleAsync(UserRole muteRole, UserContext userToUnmute, DiscordServerContext serverContext)
        {
            await _usersService.RemoveRole(muteRole, userToUnmute, serverContext);
        }
    }
}
