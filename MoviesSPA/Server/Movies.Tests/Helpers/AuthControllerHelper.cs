using Movies.Models.DTOs;

namespace Movies.Tests.Helpers;

internal static class AuthControllerHelper
{
    internal static UserDto CreateUser()
    {
        return new UserDto
        {
            Username = "123456",
            Password = "654321",
        };
    }
}
