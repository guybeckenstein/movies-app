using System.ComponentModel.DataAnnotations;

namespace Movies.Models.DTOs;

public sealed record UserDto
{
    [Required]
    public required string Username { get; init; }
    [Required]
    [MinLength(6)]
    public required string Password { get; init; }
}
