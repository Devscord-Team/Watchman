using Devscord.DiscordFramework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Commons.Extensions;
using System;

namespace Devscord.DiscordFramework.Architecture.Controllers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DiscordCommand : Attribute
    {
        public string CommandName { get; private set; }
        public DiscordCommand(string command)
        {
            this.CommandName = command;
        }

        public bool IsMatched(DiscordRequest request)
        {
            var withoutPrefix = request.OriginalMessage.CutStart(request.Prefix);
            return withoutPrefix.StartsWith(this.CommandName)
                   && (withoutPrefix.Length == this.CommandName.Length || withoutPrefix[this.CommandName.Length] == ' ');
        }
    }
}
