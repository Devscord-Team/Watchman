namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    public interface IDiscordClient
    {
        IDiscordClientUsersService UsersService { get; }
        IDiscordClientChannelsService ChannelsService { get; }
        IDiscordClientRolesService RolesService { get; }
        IDiscordClientServersService ServersService { get; }
    }
}
