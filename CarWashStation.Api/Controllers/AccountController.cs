using System.Security.Claims;
using CarWashStation.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashStation.Controllers;

[ApiController]
[Route("api/account")]
public sealed class AccountController(IConfiguration configuration) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthStatus>> Login(LoginRequest request, [FromQuery] bool useToken = false)
    {
        var email = configuration["Admin:Email"];
        var password = configuration["Admin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
            !string.Equals(request.Email, email, StringComparison.OrdinalIgnoreCase) || request.Password != password)
            return Unauthorized(new ApiMessage("Invalid login attempt."));

        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, email), new Claim(ClaimTypes.Role, "Admin")],
            CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Headers.CacheControl = "no-store";
        if (useToken)
            return SignIn(new ClaimsPrincipal(identity), BearerTokenDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = request.RememberMe });
        return Ok(new AuthStatus(true, email));
    }

    [Authorize]
    [HttpGet("status")]
    public ActionResult<AuthStatus> Status() => Ok(new AuthStatus(true, User.Identity?.Name));

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }
}
