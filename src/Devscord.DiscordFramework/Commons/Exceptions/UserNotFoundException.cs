namespace Devscord.DiscordFramework.Commons.Exceptions
{
    public class UserNotFoundException : BotException
    {
        public UserNotFoundException(string mention) : base(mention)
        {
        }
    }
}
