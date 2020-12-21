using Devscord.DiscordFramework.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Jobs
{
    public interface IHangfireJob
    {
        public Task Do();
        public RefreshFrequent Frequency { get; }
        public bool RunOnStart { get; }
    }
}
