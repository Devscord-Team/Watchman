using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using Watchman.WebClient.Areas.Auth.Discord;

namespace Watchman.WebClient.Areas.Auth
{
    public class UsersService
    {
        private static HttpClient client = new HttpClient();

        /// <summary>
        /// Parses the user's discord claim for their `identify` information
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public DiscordUserClaim GetInfo(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var claims = httpContext.User.Claims;
            bool? verified;
            if (bool.TryParse(claims.FirstOrDefault(x => x.Type == "urn:discord:verified")?.Value, out var _verified))
            {
                verified = _verified;
            }
            else
            {
                verified = null;
            }

            var userClaim = new DiscordUserClaim
            {
                UserId = ulong.Parse(claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value),
                Name = claims.First(x => x.Type == ClaimTypes.Name).Value,
                Discriminator = claims.First(x => x.Type == "urn:discord:discriminator").Value,
                Avatar = claims.First(x => x.Type == "urn:discord:avatar").Value,
                Email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                Verified = verified
            };

            return userClaim;
        }

        /// <summary>
        /// Gets the user's discord oauth2 access token
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<string> GetTokenAsync(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var tk = await httpContext.GetTokenAsync("Discord", "access_token");
            return tk;
        }

        /// <summary>
        /// Gets a list of the user's guilds, Requires `Guilds` scope
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<List<Guild>> GetUserGuildsAsync(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var token = await this.GetTokenAsync(httpContext);

            var guildEndpoint = DiscordDefaults.UserInformationEndpoint + "/guilds";

            using (var request = new HttpRequestMessage(HttpMethod.Get, guildEndpoint))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    //todo define admin user in domain
                    var guilds = Guild.ListFromJson(content);
                    var adminGuilds = guilds.Where(x => (x.Permissions & 0x20) == 0x20).ToList(); //0x20 == ManageGuild permission
                    return adminGuilds;
                }
                catch
                {
                    return null;
                }
            }
        }

        public class DiscordUserClaim
        {
            public ulong UserId { get; set; }
            public string Name { get; set; }
            public string Discriminator { get; set; }
            public string Avatar { get; set; }

            /// <summary>
            /// Will be null if the email scope is not provided
            /// </summary>
            public string Email { get; set; } = null;

            /// <summary>
            /// Whether the email on this account has been verified, can be null
            /// </summary>
            public bool? Verified { get; set; } = null;
        }

        public class Guild
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("icon")]
            public string Icon { get; set; }

            [JsonProperty("owner")]
            public bool Owner { get; set; }

            [JsonProperty("permissions")]
            public long Permissions { get; set; }

            [JsonProperty("features")]
            public List<string> Features { get; set; }

            public static List<Guild> ListFromJson(string json) => JsonConvert.DeserializeObject<List<Guild>>(json, Settings);
            private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
                {
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                },
            };
        }
    }
}