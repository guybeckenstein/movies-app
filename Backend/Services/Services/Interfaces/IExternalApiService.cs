using Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IExternalApiService
    {
        Task<List<MovieRequest>> GetMovies(string url);
    }
}
