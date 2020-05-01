namespace Devscord.DiscordFramework.Framework.Commands.Properties
{
    public enum BotCommandPropertyType
    {
        SingleWord = 0x1,
        Text = 0x2,
        Number = 0x4,
        Time = 0x8,
        UserMention = 0x16,
        ChannelMention = 0x32
    }
}
