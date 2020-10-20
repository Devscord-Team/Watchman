using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Statsman.Core.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Jobs.Statsman
{
    public class CalculateMessagesPerDay : IhangfireJob
    {
        private readonly DiscordServersService discordServersService;
        private readonly PreGeneratedStatisticsGenerator preGeneratedStatisticsGenerator;

        public CalculateMessagesPerDay(DiscordServersService discordServersService, PreGeneratedStatisticsGenerator preGeneratedStatisticsGenerator)
        {
            this.discordServersService = discordServersService;
            this.preGeneratedStatisticsGenerator = preGeneratedStatisticsGenerator;
        }

        public RefreshFrequent Frequency => RefreshFrequent.Daily;

        public async Task Do()
        {
            await foreach (var server in this.discordServersService.GetDiscordServersAsync())
            {
                await this.preGeneratedStatisticsGenerator.PreGenerateStatisticsPerDay(server.Id);
            }
        }
    }
}
