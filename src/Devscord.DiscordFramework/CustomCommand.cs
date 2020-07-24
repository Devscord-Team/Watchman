using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework
{
    public class CustomCommand
    {
        public string ExpectedBotCommandName { get; private set; } //IBotCommand
        public Regex Template { get; private set; }
        public ulong ServerId { get; private set; }

        public CustomCommand(string expectedBotCommandName, Regex template, ulong serverId)
        {
            this.ExpectedBotCommandName = expectedBotCommandName;
            this.Template = template;
            this.ServerId = serverId;
        }
    }
}
