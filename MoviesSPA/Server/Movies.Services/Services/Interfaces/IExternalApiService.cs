using Movies.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Services.Services.Interfaces;

public interface IExternalApiService
{
    Task<IEnumerable<MovieResponse>> GetMovies(string url);
}
