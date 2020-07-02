namespace Devscord.DiscordFramework.Commons.Exceptions
{
    public class RoleNotFoundException : BotException
    {
        public RoleNotFoundException(string roleName) : base(roleName)
        {
        }
    }
}
