using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Statsman.Core.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Jobs.Statsman
{
    public class CalculateMessagesPerMonth : IhangfireJob
    {
        private readonly DiscordServersService discordServersService;
        private readonly PreReneratedStatisticsGenerator preReneratedStatisticsGenerator;

        public CalculateMessagesPerMonth(DiscordServersService discordServersService, PreReneratedStatisticsGenerator preReneratedStatisticsGenerator)
        {
            this.discordServersService = discordServersService;
            this.preReneratedStatisticsGenerator = preReneratedStatisticsGenerator;
        }

        public RefreshFrequent Frequency => RefreshFrequent.Weekly;

        public async Task Do()
        {
            await foreach (var server in this.discordServersService.GetDiscordServersAsync())
            {
                await this.preReneratedStatisticsGenerator.PreGenerateStatisticsPerMonth(server.Id);
            }
        }
    }
}
