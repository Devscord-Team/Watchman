using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.Discord.Areas.Initialization.Services;

namespace Watchman.Discord.Areas.Initialization.Controllers
{
    public class InitializationController : IController
    {
        private readonly InitializationService _initializationService;

        public InitializationController(InitializationService initializationService)
        {
            _initializationService = initializationService;
        }

        [AdminCommand]
        [DiscordCommand("init")]
        //[IgnoreForHelp] TODO //TODO co to za TODO?
        public async Task Init(DiscordRequest request, Contexts contexts)
        {
            await _initializationService.InitServer(contexts.Server);
        }
    }
}
