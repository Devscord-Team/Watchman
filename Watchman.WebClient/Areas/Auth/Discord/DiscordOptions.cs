using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

namespace Watchman.WebClient.Areas.Auth.Discord
{
    /// <summary> Configuration options for <see cref="DiscordHandler"/>. </summary>
    public class DiscordOptions : OAuthOptions
    {
        /// <summary> Initializes a new <see cref="DiscordOptions"/>. </summary>
        public DiscordOptions()
        {
            this.CallbackPath = new PathString("/signin-discord");
            this.AuthorizationEndpoint = DiscordDefaults.AuthorizationEndpoint;
            this.TokenEndpoint = DiscordDefaults.TokenEndpoint;
            this.UserInformationEndpoint = DiscordDefaults.UserInformationEndpoint;
            this.Scope.Add("identify");

            this.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id", ClaimValueTypes.UInteger64);
            this.ClaimActions.MapJsonKey(ClaimTypes.Name, "username", ClaimValueTypes.String);
            this.ClaimActions.MapJsonKey(ClaimTypes.Email, "email", ClaimValueTypes.Email);
            this.ClaimActions.MapJsonKey("urn:discord:discriminator", "discriminator", ClaimValueTypes.UInteger32);
            this.ClaimActions.MapJsonKey("urn:discord:avatar", "avatar", ClaimValueTypes.String);
            this.ClaimActions.MapJsonKey("urn:discord:verified", "verified", ClaimValueTypes.Boolean);
        }

        /// <summary> Gets or sets the Discord-assigned appId. </summary>
        public string AppId { get => this.ClientId; set => this.ClientId = value; }
        /// <summary> Gets or sets the Discord-assigned app secret. </summary>
        public string AppSecret { get => this.ClientSecret; set => this.ClientSecret = value; }
    }
}
