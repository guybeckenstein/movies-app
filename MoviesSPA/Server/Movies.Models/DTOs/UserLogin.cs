using System.ComponentModel.DataAnnotations;

namespace Movies.Data.DTOs;

public sealed record UserLogin
{
    [Required]
    public required string Username { get; init; }
    [Required]
    [MinLength(6)]
    public required string Password { get; init; }
    [Required]
    public required bool RememberMe { get; init; }
}
