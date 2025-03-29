using System.ComponentModel.DataAnnotations;

namespace Movies.Data.DTOs;

public sealed record UserRegistration
{
    [Required]
    public required string Username { get; init; }
    [Required]
    [MinLength(6)]
    public required string Password { get; init; }
    [Required]
    public string Email { get; init; }
    [Required]
    public string PhoneNumber { get; init; }
    [Required]
    public string FirstName { get; init; }
    [Required]
    public string LastName { get; init; }
}
