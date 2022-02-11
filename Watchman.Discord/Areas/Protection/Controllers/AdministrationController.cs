using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Protection.BotCommands;
using Watchman.Discord.Areas.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AdministrationController : IController
    {
        private readonly IMessagesServiceFactory _messagesServiceFactory;
        private readonly ComplaintsChannelService _complaintsChannelService;

        public AdministrationController(IMessagesServiceFactory messagesServiceFactory, ComplaintsChannelService complaintsChannelService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._complaintsChannelService = complaintsChannelService;
        }

        [AdminCommand]
        public async Task CreateChannelForComplaints(ComplaintsChannelCommand command, Contexts contexts)
        {
            var complaintsChannelName = string.IsNullOrWhiteSpace(command.Name) ? "skargi" : command.Name;
            _ = await this._complaintsChannelService.CreateComplaintsChannel(complaintsChannelName, contexts);
            var messagesService = this._messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.ComplaintsChannelHasBeenCreated());
        }
    }
}
