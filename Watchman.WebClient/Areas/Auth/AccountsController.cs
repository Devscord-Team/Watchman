using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace Watchman.WebClient.Areas.Auth
{
    [Route("[controller]/[action]")]
    public class AccountsController : ControllerBase
    {
        public IDataProtectionProvider Provider { get; }

        public AccountsController(IDataProtectionProvider provider)
        {
            this.Provider = provider;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return this.Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "Discord");
        }

        [HttpGet]
        public async Task<IActionResult> LogOut(string returnUrl = "/")
        {
            //This removes the cookie assigned to the user login.
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.LocalRedirect(returnUrl);
        }
    }
}