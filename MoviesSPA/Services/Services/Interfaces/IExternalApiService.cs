using Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IExternalApiService
    {
        Task<List<MovieResponse>> GetMovies(string url);
    }
}
