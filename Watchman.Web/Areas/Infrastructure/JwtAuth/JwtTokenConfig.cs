using System.Text.Json.Serialization;

namespace Watchman.Web.Areas.Infrastructure.JwtAuth
{
    public class JwtTokenConfig
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int AccessTokenExpiration { get; set; }

        public int RefreshTokenExpiration { get; set; }
    }
}
