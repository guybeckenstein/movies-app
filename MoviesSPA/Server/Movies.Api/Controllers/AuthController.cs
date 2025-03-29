using Microsoft.AspNetCore.Mvc;
using Movies.Api.Filters.ActionFilters;
using Movies.Data.DTOs;
using Movies.Services.Services.Interfaces;

namespace Movies.Api.Controllers;

[RedirectionFilter]
public sealed class AuthController(IConfiguration configuration, IAuthService authService) : BaseController(configuration)
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register", Name = "register")]
    public async Task<IActionResult> Register([FromBody] UserRegistration user)
    {
        var res = await _authService.Register(user);
        // You can only register once in this project logic
        if (res is null)
        {
            return Redirect("/api/Movies/"); // TODO: create specific status code for an existing user scenario
        }
        else if (res is false)
        {
            return BadRequest(); // TODO: create specific status code for database operation failure scenario
        }
        return Ok(); // TODO: create specific status code for successful request
    }

    [HttpPost("login", Name = "login")]
    public async Task<IActionResult> Login([FromBody] UserLogin user)
    {
        var jwtSettingsToken = _configuration["JwtSettings:Token"] 
            ?? throw new KeyNotFoundException("configuration 'JwtSettings:Token' does not exist in appsettings.json");
        var cookieToken = await _authService.Login(user, jwtSettingsToken); // Validate login and then create new JWT token
        // You can only login once (with the correct user!!) in this project logic
        if (cookieToken is null)
        {
            return BadRequest(); // TODO: create specific status code for invalid credentials scenario
        }
        else if (cookieToken.Equals(""))
        {
            return BadRequest(); // TODO: create specific status code for database operation failure scenario
        }

        AppendAccessToken(cookieToken);
        return Ok(); // TODO: create specific status code for successful request
    }

    private void AppendAccessToken(string cookieToken)
    {
        // Set the cookie with the JWT token
        var jwtCookieName = _configuration["JwtSettings:CookieName"]
            ?? throw new KeyNotFoundException("configuration 'JwtSettings:CookieName' does not exist in appsettings.json");

        Response.Cookies.Append(
            jwtCookieName,
            cookieToken,
            new CookieOptions
            {
                #if DEBUG
                HttpOnly = false,
                SameSite = SameSiteMode.None,
                #else
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                #endif
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });
    }
}
