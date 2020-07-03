using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.UselessFeatures.BotCommands;

// do obsługi reakcji:
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models; 
using System.Linq;
using Devscord.DiscordFramework.Services;
//

namespace Watchman.Discord.Areas.UselessFeatures.Controllers
{
    public class UselessController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UsersService _usersService;
        //
        private readonly UsersRolesService _usersRolesService;
        private readonly ReactionsService _reactionsService;
        //

        public UselessController(MessagesServiceFactory messagesServiceFactory, UsersService usersService, UsersRolesService usersRolesService, ReactionsService reactionsService)
        {
            _messagesServiceFactory = messagesServiceFactory;
            _usersService = usersService;
            //
            _usersRolesService = usersRolesService;
            _reactionsService = reactionsService;
            //
        }

        public async Task PrintMarchew(MarchewCommand command, Contexts contexts)
        {
            const string text = "Moim zdaniem to nie ma tak, że coś jest programowaniem, albo nie jest programowaniem. Gdybym miał powiedzieć, co cenię w programowaniu najbardziej, powiedziałbym, że ludzi. Ekhm… Ludzi, którzy podali mi pomocną dokumentacje, kiedy sobie nie radziłem, kiedy byłem sam. I co ciekawe, to właśnie przypadkowe spotkania wpływają na nasze życie. Chodzi o to, że kiedy wyznaje się pewne wartości, nawet pozornie uniwersalne, bywa, że nie znajduje się zrozumienia, które by tak rzec, które pomaga się nam rozwijać. Ja miałem szczęście, by tak rzec, ponieważ je znalazłem. I dziękuję życiu. Dziękuję mu, życie to śpiew, życie to taniec, życie to miłość. Wielu ludzi pyta mnie o to samo, ale jak ty to robisz? Skąd czerpiesz tę radość? A ja odpowiadam, że to proste, to umiłowanie życia, to właśnie ono sprawia, że dzisiaj na przykład programuje nawigacje, a jutro… kto wie, dlaczego by nie, oddam się pracy społecznej i będę ot, choćby uczyć… znaczy… juniorów.";
            var messagesService = _messagesServiceFactory.Create(contexts);
            await messagesService.SendMessage(text);
        }

        // do testu obsługi reackji
        // ta komenda z parametrem "on" aktywuje możliwość przyznania roli python i usunięcia tej roli gdy zareaguje się na konkretną wiadomość (w sensie o konkretnej treści)
        [DiscordCommand("MessageRoleOnRequest")]
        public void AddRoleOnRequest(DiscordRequest request, Contexts contexts)
        {
            if (request.Arguments?.FirstOrDefault()?.Name == "on")
            {
                _reactionsService.AddOnUserAddedReaction(rc => 
                {
                    if (rc.MessageContext.Content.Trim() == "Like = dostajesz role python" && rc.EmoteName == "👍") 
                    {
                        var pythonRole = _usersRolesService.GetRoleByName("python", rc.Contexts.Server);
                        _usersService.AddRole(pythonRole, rc.Contexts.User, rc.Contexts.Server).Wait();
                    }        
                });

                _reactionsService.AddOnUserRemovedReaction(rc => 
                {
                    if (rc.MessageContext.Content.Trim() == "Like = tracisz role python" && rc.EmoteName == "👍")
                    {
                        var pythonRole = _usersRolesService.GetRoleByName("python", rc.Contexts.Server);
                        _usersService.RemoveRole(pythonRole, rc.Contexts.User, rc.Contexts.Server).Wait();
                    }
                });
            }
        }
        //
    }
}
