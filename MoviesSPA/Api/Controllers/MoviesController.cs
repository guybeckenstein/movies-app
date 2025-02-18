using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;

namespace Api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IExternalApiService _externalApiService;

        public MoviesController(IConfiguration configuration, 
            IExternalApiService externalApiService)
        {
            _configuration = configuration;
            _externalApiService = externalApiService;
        }

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
