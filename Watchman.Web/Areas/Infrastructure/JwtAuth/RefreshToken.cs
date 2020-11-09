using System;
using System.Text.Json.Serialization;

namespace Watchman.Web.Areas.Infrastructure.JwtAuth
{
    public class RefreshToken
    {
        public string UserName { get; set; }   

        public string TokenString { get; set; }

        public DateTime ExpireAt { get; set; }
    }
}
