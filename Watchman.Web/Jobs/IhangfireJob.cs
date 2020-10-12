using Devscord.DiscordFramework.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Jobs
{
    public interface IhangfireJob
    {
        public Task Do();
        public RefreshFrequent Frequency { get; }
    }
}
