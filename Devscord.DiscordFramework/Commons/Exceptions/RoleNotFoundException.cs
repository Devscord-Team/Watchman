namespace Devscord.DiscordFramework.Commons.Exceptions
{
    public class RoleNotFoundException : BotException
    {
<<<<<<< HEAD
        public string RoleName { get; }

        public RoleNotFoundException(string roleName) => this.RoleName = roleName;
=======
        public RoleNotFoundException(string roleName) : base(roleName)
        {
        }
>>>>>>> master
    }
}
