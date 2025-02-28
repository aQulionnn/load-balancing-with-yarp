using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apigateway.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet("login")]
    public IActionResult Login()
    {
        return SignIn(
            new ClaimsPrincipal (
                new ClaimsIdentity(
                    [
                        new Claim("sub", Guid.NewGuid().ToString())
                    ],
                    BearerTokenDefaults.AuthenticationScheme
                )
            ),
            authenticationScheme: BearerTokenDefaults.AuthenticationScheme
        );
    }
}