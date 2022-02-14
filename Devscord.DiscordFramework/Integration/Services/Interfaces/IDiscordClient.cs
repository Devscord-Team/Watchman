namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    public interface IDiscordClient
    {
        IDiscordClientMessagesService MessagesService { get; }
        IDiscordClientUsersService UsersService { get; }
        IDiscordClientChannelsService ChannelsService { get; }
        IDiscordClientRolesService RolesService { get; }
        IDiscordClientServersService ServersService { get; }
    }
}
