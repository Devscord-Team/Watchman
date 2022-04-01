using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Commands.Responses
{
    public interface IResponsesService
    {
        IEnumerable<Response> Responses { get; set; }
        string GetResponse(ulong serverId, Func<IResponsesService, string> getResponse);
        string ProcessResponse(string response, params KeyValuePair<string, string>[] values);
        string ProcessResponse(string response, Contexts contexts, params KeyValuePair<string, string>[] values);
    }
}
