using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Models.Models;
using Services.Services.Interfaces;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration configuration,
            IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
        }

        [HttpPost("register", Name = "register")]
        public ActionResult<User> Register([FromBody] UserDto user)
        {
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
            // Checks if there is already a JWT token cookie
            if (Request.Cookies.TryGetValue("jwt", out var token))
            {
                return Redirect("/api/Movies/");
            }

            var jwtSettingsToken = _configuration["JwtSettings:Token"]!; // Get token from `appsettings.json` file
            var cookieToken = _authService.Login(user, jwtSettingsToken); // Validate login and then create new JWT token
            // You can only login once (with the correct user!!) in this project logic
            if (cookieToken is null)
            {
                return BadRequest("Wrong username or password");
            }
            // Set the cookie with the JWT token
            Response.Cookies.Append("jwt", cookieToken, new CookieOptions
            {
                #if (DEBUG)
                    HttpOnly = false,
                    SameSite = SameSiteMode.None,
                    Secure = false,
                #else
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = true,
                #endif
                Expires = DateTime.UtcNow.AddDays(1)
            });
            return Ok(cookieToken);
        }
    }
}
