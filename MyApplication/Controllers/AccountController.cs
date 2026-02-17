using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace MyApplication.Controllers;

[ApiController]
[Route("account")]
public class AccountController : Controller
{
    [HttpGet("login")]
    public IActionResult Login([FromQuery] string? returnUrl = "/")
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            returnUrl = "/";

        return Challenge(new AuthenticationProperties
        {
            RedirectUri = returnUrl
        });
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout([FromQuery] string? returnUrl = "/")
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            returnUrl = "/";

        var hasOidc = (await HttpContext.RequestServices
            .GetRequiredService<IAuthenticationSchemeProvider>()
            .GetSchemeAsync(OpenIdConnectDefaults.AuthenticationScheme)) is not null;

        if (!hasOidc)
            return Redirect(returnUrl);

        return SignOut(
            new AuthenticationProperties { RedirectUri = returnUrl },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}
