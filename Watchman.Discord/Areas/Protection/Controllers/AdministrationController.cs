using System.Threading.Tasks;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Protection.BotCommands;
using Watchman.Discord.Areas.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AdministrationController : IController
    {
        private readonly IMessagesServiceFactory _messagesServiceFactory;
        private readonly IComplaintsChannelService _complaintsChannelService;

        public AdministrationController(IMessagesServiceFactory messagesServiceFactory, IComplaintsChannelService complaintsChannelService)
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
