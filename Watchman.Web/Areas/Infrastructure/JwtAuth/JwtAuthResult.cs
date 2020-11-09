using System.Text.Json.Serialization;

namespace Watchman.Web.Areas.Infrastructure.JwtAuth
{
    public class JwtAuthResult
    {
        public string AccessToken { get; set; }

        public RefreshToken RefreshToken { get; set; }
    }
}
