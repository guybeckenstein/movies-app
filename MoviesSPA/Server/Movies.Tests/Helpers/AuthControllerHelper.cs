using Movies.Data.DTOs;

namespace Movies.Tests.Helpers;

internal static class AuthControllerHelper
{
    internal static UserLogin CreateUser()
    {
        return new UserLogin
        {
            Username = "123456",
            Password = "654321",
            RememberMe = false,
        };
    }
}
