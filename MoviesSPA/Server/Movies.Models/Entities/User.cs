#nullable enable
using System;

namespace Movies.Data.Entities;

public sealed class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public string Password { get; set; }
    public required string Email { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
}
