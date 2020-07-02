namespace Devscord.DiscordFramework.Commons.Exceptions
{
    public class UserNotFoundException : BotException
    {
<<<<<<< HEAD
        public string Mention { get; }

        public UserNotFoundException(string mention) => this.Mention = mention;
=======
        public UserNotFoundException(string mention) : base(mention)
        {
        }
>>>>>>> master
    }
}
