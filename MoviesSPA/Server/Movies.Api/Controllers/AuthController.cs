using Microsoft.AspNetCore.Mvc;
using Movies.Models.DTOs;
using Movies.Models.Models;
using Movies.Services.Services.Interfaces;

namespace Movies.Api.Controllers;

public sealed class AuthController(IConfiguration configuration, IAuthService authService) : BaseController(configuration)
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register", Name = "register")]
    public ActionResult<User> Register([FromBody] UserDto user)
    {
        if (user is null)
        {
            return BadRequest("Request body cannot be empty.");
        }
        // Checks if there is already a JWT token cookie
        if (Request.Cookies.TryGetValue("jwt", out var token))
        {
            return Redirect("/api/Movies/");
        }

        var res = _authService.Register(user);
        // You can only register once in this project logic
        if (res is null)
        {
            return Redirect("/api/Movies/");
        }
        return Ok(res);
    }

    [HttpPost("login", Name = "login")]
    public ActionResult<User> Login([FromBody] UserDto user)
    {
        if (user is null)
        {
            return BadRequest("Request body cannot be empty.");
        }
        // Checks if there is already a JWT token cookie
        if (Request.Cookies.TryGetValue("jwt", out var token))
        {
            return Redirect("/api/Movies/");
        }

        var jwtSettingsToken = _configuration["JwtSettings:Token"] 
            ?? throw new KeyNotFoundException("configuration 'JwtSettings:Token' does not exist in appsettings.json");
        var cookieToken = _authService.Login(user, jwtSettingsToken); // Validate login and then create new JWT token
        // You can only login once (with the correct user!!) in this project logic
        if (cookieToken is null)
        {
            return BadRequest("Wrong username or password");
        }
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
                Expires = DateTime.UtcNow.AddDays(1)
            });
        return Ok(cookieToken);
    }
}
