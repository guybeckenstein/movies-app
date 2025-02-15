#nullable enable
using Models.DTOs;
using Models.Models;

namespace Services.Services.Interfaces
{
    public interface IAuthService
    {
        User? Register(UserDto user);
        string? Login(UserDto user, string token);
    }
}
