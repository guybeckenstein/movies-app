using Microsoft.AspNetCore.Mvc;
using Movies.Services.Services.Interfaces;

namespace Movies.Api.Controllers;

public sealed class MoviesController(IConfiguration configuration, IExternalApiService externalApiService) : BaseController(configuration)
{
    private readonly IExternalApiService _externalApiService = externalApiService;

    [HttpGet(Name = "/")]
    public async Task<IActionResult> GetMoviesAsync()
    {
        var moviesApiUrl = _configuration["ExternalLinks:Data:Movies"] 
            ?? throw new KeyNotFoundException($"configuration 'ExternalLinks:Data:Movies' does not exist in appsettings.json");

        var res = await _externalApiService.GetMovies(moviesApiUrl);

        return Ok(res);
    }
}
