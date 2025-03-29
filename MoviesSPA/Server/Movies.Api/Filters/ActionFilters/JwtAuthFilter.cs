using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies.Data.Entities;
using Movies.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Movies.Api.Filters.ActionFilters;

public class JwtAuthFilter(IConfiguration configuration, MyDbContext dbContext) : IAsyncActionFilter
{
    private readonly IConfiguration _configuration = configuration;
    private readonly MyDbContext _dbContext = dbContext;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var jwtCookieName = _configuration["JwtSettings:CookieName"] 
            ?? throw new KeyNotFoundException("configuration 'JwtSettings:CookieName' does not exist in appsettings.json");

        // Step 1: Check for existing JWT token
        if (context.HttpContext.Request.Cookies.TryGetValue(jwtCookieName, out var token))
        {
            var principal = ValidateToken(token, out bool isExpired);

            if (principal is not null && isExpired is false)
            {
                // JWT is valid -> Continue request
                await next();
                return;
            }

            if (isExpired)
            {
                // Step 2: Refresh token logic
                var username = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (username == null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var user = await _dbContext.Users
                    .Where(u => u.Username.ToString() == username)
                    .FirstOrDefaultAsync();

                if (user == null || string.IsNullOrEmpty(user.RefreshToken) || user.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    // No valid refresh token, reject request
                    context.Result = new UnauthorizedResult();
                    return;
                }

                // Generate new access token, then append it
                var newToken = GenerateJwtToken(user);
                context.HttpContext.Response.Cookies.Append(jwtCookieName, newToken, GetCookieOptions());

                await next();
                return;
            }
        }

        // No token found, redirect
        context.Result = new RedirectResult("/api/Movies/");
    }

    private ClaimsPrincipal? ValidateToken(string token, out bool isExpired)
    {
        isExpired = false;
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Token"]
            ?? throw new KeyNotFoundException("configuration 'JwtSettings:Token' does not exist in appsettings.json"));

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true
            }, out var validatedToken);

            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            isExpired = true;
            return null;
        }
        catch
        {
            return null;
        }
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Token"]
            ?? throw new KeyNotFoundException("configuration 'JwtSettings:Token' does not exist in appsettings.json")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
        };

        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds,
            claims: claims);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private CookieOptions GetCookieOptions()
    {
        return new CookieOptions
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
        };
    }
}
