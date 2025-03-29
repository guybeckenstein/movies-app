using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Movies.Data.Models;

public sealed record MovieRequest : IValidatableObject
{
    [Required]
    public required string Title { get; init; }
    [Required]
    [StringLength(4, MinimumLength = 4, ErrorMessage = "The year must be a string with the exact length of 4")]
    public required string Year { get; init; }
    [Required]
    public required string Rated { get; init; }
    public required string Released { get; init; }
    public required string Runtime { get; init; }
    [Required]
    public required string Genre { get; init; }
    public required string Director { get; init; }
    public required string Writer { get; init; }
    public required string Actors { get; init; }
    public required string Plot { get; init; }
    public required string Language { get; init; }
    public required string Country { get; init; }
    public required string Awards { get; init; }
    [Required]
    public required string Poster { get; init; }
    public required string ImdbRating { get; init; }
    public required string ImdbVotes { get; init; }
    public required string ImdbID { get; init; }
    public required string Type { get; init; }
    public required string Response { get; init; }
    public required List<string> Images { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Released.Contains(Year))
        {
            yield return new ValidationResult("Release date attribute must have same year", [nameof(Released)]);
        }
    }
}
