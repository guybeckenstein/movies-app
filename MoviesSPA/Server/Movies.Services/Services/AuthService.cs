#nullable enable
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Movies.Data.DTOs;
using Movies.Data.Entities;
using Movies.Data.Models;
using Movies.Data.Repositories.Interfaces;
using Movies.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Services.Services;

public sealed class AuthService(ILogger<IAuthService> logger, IUserRepository userRepository) : BaseService<IAuthService>(logger), IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    public async Task<bool?> Register(UserRegistration model)
    {
        _logger.LogInformation($"AuthService.Register - Starting request for {model.Username}");

        var baseUser = new BaseUser(model.Username, model.Password); 
        if ((await _userRepository.Find(baseUser)) is not null)
        {
            _logger.LogError($"AuthService.Register - User {model.Username} already registered! Redirecting request");
            return null;
        }

        var newUser = new User
        {
            Username = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };
        newUser.Password = new PasswordHasher<User>().HashPassword(newUser, model.Password);

        var isUserAdded = await _userRepository.Add(newUser);
        _logger.LogInformation($"AuthService.Register - Finishing request for {model.Username}");
        if (isUserAdded)
        {
            _logger.LogDebug($"AuthService.Register - Added new user {model.Username} successfully");
        }
        else
        {
            _logger.LogDebug($"AuthService.Register - Adding new user {model.Username} failed");
        }

        return isUserAdded;
    }
    public async Task<string?> Login(UserLogin model, string token)
    {
        _logger.LogInformation($"AuthService.Login - Starting request for {model.Username}");

        var baseUser = new BaseUser(model.Username, model.Password);
        var user = await _userRepository.Find(baseUser);
        if (user is null)
        {
            _logger.LogError($"AuthService.Login - User {model.Username} not registered!");
            return null;
        }

        if (model.RememberMe is true && (user.RefreshToken is null || user.RefreshTokenExpiry < DateTime.UtcNow))
        {
            // Generate a new refresh token with 7 days expiry
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            var isRefreshTokenAdded = await _userRepository.Update(user);
            if (isRefreshTokenAdded)
            {
                _logger.LogDebug($"AuthService.Register - Updated user {model.Username} refresh token successfully");
            }
            else
            {
                _logger.LogDebug($"AuthService.Register - Updating user {model.Username} refresh token failed");
                return "";
            }
        }

        var res = GenerateAccessToken(token, user.Username);
        _logger.LogInformation($"AuthService.Login - Generated access token for {model.Username}");
        return res;
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    private string GenerateAccessToken(string token, string username)
    {
        _logger.LogInformation($"AuthService.CreateToken - Starting request for {username}");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: "http://localhost:4200",
            audience: "http://localhost:4200",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
