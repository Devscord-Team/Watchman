namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    internal interface IDiscordClient
    {
        IDiscordClientUsersService UsersService { get; }
        IDiscordClientChannelsService ChannelsService { get; }
        IDiscordClientRolesService RolesService { get; }
        IDiscordClientServersService ServersService { get; }
    }
}
