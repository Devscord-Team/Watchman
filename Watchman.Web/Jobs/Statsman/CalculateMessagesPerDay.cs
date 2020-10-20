using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Statsman.Core.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Jobs.Statsman
{
    public class CalculateMessagesPerDay : IHangfireJob
    {
        private readonly DiscordServersService _discordServersService;
        private readonly PreGeneratedStatisticsGenerator _preGeneratedStatisticsGenerator;

        public CalculateMessagesPerDay(DiscordServersService discordServersService, PreGeneratedStatisticsGenerator preGeneratedStatisticsGenerator)
        {
            this._discordServersService = discordServersService;
            this._preGeneratedStatisticsGenerator = preGeneratedStatisticsGenerator;
        }

        public RefreshFrequent Frequency => RefreshFrequent.Daily;

        public async Task Do()
        {
            await foreach (var server in this._discordServersService.GetDiscordServersAsync())
            {
                await this._preGeneratedStatisticsGenerator.PreGenerateStatisticsPerDay(server.Id);
            }
        }
    }
}
