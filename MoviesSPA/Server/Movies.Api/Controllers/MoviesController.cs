using Microsoft.AspNetCore.Mvc;
using Movies.Services.Services.Interfaces;

namespace Movies.Api.Controllers
{
    [Route("api/[controller]")]
    public class MoviesController : BaseController
    {
        private readonly IExternalApiService _externalApiService;

        public MoviesController(IConfiguration configuration, IExternalApiService externalApiService) : base(configuration) => _externalApiService = externalApiService;

        [HttpGet(Name = "/")]
        public async Task<IActionResult> GetMoviesAsync()
        {
            var moviesApiUrl = _configuration["ExternalLinks:Data:Movies"] 
                ?? throw new ArgumentNullException($"configuration 'ExternalLinks:Data:Movies' does not exist in appsettings.json");

            var res = await _externalApiService.GetMovies(moviesApiUrl);

            return Ok(res);
        }
    }
}
