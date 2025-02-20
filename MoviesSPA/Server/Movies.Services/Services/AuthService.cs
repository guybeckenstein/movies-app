#nullable enable
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Movies.Models.DTOs;
using Movies.Models.Models;
using Movies.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Movies.Services.Services
{
    public class AuthService : BaseService<IAuthService>, IAuthService
    {
        private readonly static User _user;
        private readonly static string _jwtToken;

        public AuthService(ILogger<IAuthService> logger) : base(logger) { }
        static AuthService()
        {
            _user = new User();
            _jwtToken = "";
        }

        public User? Register(UserDto user)
        {
            _logger.LogInformation($"AuthService.Register - Starting request for {user.Username}");
            if (!(_user.Username is null || _user.PasswordHash is null))
            {
                _logger.LogError($"AuthService.Register - User {user.Username} already registered! Redirecting request");
                return null;
            }
            var hashedPassword = new PasswordHasher<User>().HashPassword(_user, user.Password);

            _user.Username = user.Username;
            _user.PasswordHash = hashedPassword;

            _logger.LogInformation($"AuthService.Register - Finishing request for {user.Username}");
            return _user;
        }
        public string? Login(UserDto user, string token)
        {
            _logger.LogInformation($"AuthService.Login - Starting request for {user.Username}");
            if (_user.Username != user.Username ||
                new PasswordHasher<User>().VerifyHashedPassword(_user, _user.PasswordHash, user.Password) == PasswordVerificationResult.Failed)
            {
                _logger.LogError($"AuthService.Login - User {user.Username} already logged in! Bad request");
                return null;
            }

            var res = CreateToken(token);
            _logger.LogInformation($"AuthService.Login - Generated token for {user.Username}");
            return res;
        }

        private string CreateToken(string token)
        {
            _logger.LogInformation($"AuthService.CreateToken - Starting request for {_user.Username}");
            if (string.IsNullOrEmpty(_jwtToken))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, _user.Username)
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
            _logger.LogInformation($"AuthService.CreateToken - Generated token for {_user.Username}");
            return _jwtToken;
        }
    }
}
