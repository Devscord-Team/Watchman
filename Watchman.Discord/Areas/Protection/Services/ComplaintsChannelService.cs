using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.Cqrs;
using Watchman.DomainModel.Protection.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class ComplaintsChannelService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public ComplaintsChannelService(IQueryBus queryBus, ICommandBus commandBus)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
        }

        public async Task RemoveIfNeededComplaintsChannel(ChannelContext channel, DiscordServerContext server)
        {
            var query = new GetComplaintsChannelQuery(server.Id);
            var complaintsChannel = this._queryBus.Execute(query).ComplaintsChannel;
        }
    }
}
