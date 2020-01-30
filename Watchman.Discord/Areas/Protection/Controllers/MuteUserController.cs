using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Mute;
using Watchman.DomainModel.Mute.Commands;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class MuteUserController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UsersRolesService _usersRolesService;
        private readonly UsersService _usersService;

        public MuteUserController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory, UsersRolesService usersRolesService, UsersService usersService)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _messagesServiceFactory = messagesServiceFactory;
            _usersRolesService = usersRolesService;
            _usersService = usersService;
        }

        [DiscordCommand("mute")]
        public void MuteUser(DiscordRequest request, Contexts contexts)
        {
            var messagesService = _messagesServiceFactory.Create(contexts);

            var mention = request.Arguments.ToList().FirstOrDefault()?.Values.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(mention)) //todo: przenieść do responseService
            {
                messagesService.SendMessage("Musisz wskazać użytkownika do zmutowania");
                return;
            }

            var userToMute = FindUserByMention(mention, contexts.Server);

            if (userToMute == null)
            {
                messagesService.SendMessage("Użytkownik nie istnieje");
                return;
            }

            contexts.SetContext(userToMute);
            var muteRole = GetMuteRole(contexts);

            if (muteRole == null)
            {
                messagesService.SendMessage("Rola muted nie istnieje");
                return;
            }

            var muteEvent = CreateMuteEvent(contexts.User.Id, request);

            MuteUser(contexts, muteRole);
            messagesService.SendMessage($"Użytkownik został zmutowany do {muteEvent.TimeRange.End}");

            _commandBus.ExecuteAsync(new AddMuteInfoToDbCommand(muteEvent));

            Task.Delay(muteEvent.TimeRange.End - muteEvent.TimeRange.Start).Wait();

            UnmuteUser(contexts, muteRole);
            messagesService.SendMessage("Użytkownik może pisać ponownie");
        }

        private UserContext FindUserByMention(string mention, DiscordServerContext server)
        {
            return _usersService.GetUsers(server).FirstOrDefault(x => x.Mention == mention);
        }

        private UserRole GetMuteRole(Contexts contexts)
        {
            return _usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, contexts.Server);
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

        private void MuteUser(Contexts contexts, UserRole muteRole)
        {
            _usersService.AddRole(muteRole, contexts.User, contexts.Server);
        }

        private void UnmuteUser(Contexts contexts, UserRole muteRole)
        {
            _usersService.RemoveRole(muteRole, contexts.User, contexts.Server);
        }
    }
}
