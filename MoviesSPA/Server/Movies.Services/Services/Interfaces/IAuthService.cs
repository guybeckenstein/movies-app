#nullable enable
using Movies.Data.DTOs;
using System.Threading.Tasks;

namespace Movies.Services.Services.Interfaces;

public interface IAuthService
{
    Task<bool?> Register(UserRegistration model);
    Task<string?> Login(UserLogin model, string token);
}
