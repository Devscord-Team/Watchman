namespace Devscord.DiscordFramework.Commons.Exceptions
{
    public class ChannelNotFoundException : BotException
    {
        public ChannelNotFoundException(string mention) : base(mention)
        {

        }
    }
}