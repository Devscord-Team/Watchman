using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Devscord.DiscordFramework.UnitTests.Commands.RunnerOfIBotCommandMethodsTests.BotCommands
{
    public class ChannelMentionCommand : IBotCommand
    {
        [ChannelMention]
        public ulong TestChannelMention { get; set; }
    }
}
