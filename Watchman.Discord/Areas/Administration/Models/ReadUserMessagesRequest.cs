using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Watchman.Common.Models;

namespace Watchman.Discord.Areas.Administration.Models
{
    public class ReadUserMessagesRequest
    {
        public string Mention { get; private set; }
        public int MinutesSince { get; private set; } = 15;

        public ReadUserMessagesRequest(IEnumerable<DiscordRequestArgument> arguments)
        {
            this.Mention = arguments.FirstOrDefault(x => x.Value.StartsWith('<') && x.Value.EndsWith('>'))?.Value;
            var minutesSince = arguments.FirstOrDefault(x => x.Name == "t" || x.Name == "time")?.Value;
            if (int.TryParse(minutesSince, out var result))
            {
                this.MinutesSince = result;
            }
        }

        public TimeRange GetTimeRange()
        {
            return TimeRange.Create(DateTime.UtcNow.AddMinutes(-this.MinutesSince), DateTime.UtcNow);
        }
    }
}
