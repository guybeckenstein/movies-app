#nullable enable
using Movies.Models.DTOs;
using Movies.Models.Models;

namespace Movies.Services.Services.Interfaces;

public interface IAuthService
{
    User? Register(UserDto user);
    string? Login(UserDto user, string token);
}
