using System.Collections.Generic;

namespace Movies.Models.DTOs;

public sealed record MovieResponse
{
    public required string Title { get; init; }
    public required string Year { get; init; }
    public required List<string> Genres { get; init; }
    public required string PosterImg { get; init; }
}
