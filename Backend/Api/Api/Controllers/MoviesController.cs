using Api.DTOs;
using Api.Profiles;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Services.Services.Interfaces;

namespace Api.Controllers
{

    // TODO - return 400 when unauthorized and not 500
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IConfiguration _configuration;
        private readonly IExternalApiService _externalApiService;

        public MoviesController(IMapper mapper, IConfiguration configuration, 
            IExternalApiService externalApiService)
        {
            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapMethod = mapper => false;
                cfg.CreateMap<MovieRequest, MovieResponse>();
                cfg.AddProfile<MovieProfile>();
                cfg.ReplaceMemberName(nameof(MovieRequest.Poster), nameof(MovieResponse.Poster));
                cfg.ReplaceMemberName(nameof(MovieRequest.Genre), nameof(MovieResponse.Genres));
            });

            _mapper = mapper;
            _configuration = configuration;
            _externalApiService = externalApiService;
        }

        [HttpGet(Name = "/")]
        public async Task<IActionResult> GetMoviesAsync()
        {
            var moviesApiUrl = _configuration["ExternalLinks:Data:Movies"] 
                ?? throw new ArgumentNullException($"configuration 'ExternalLinks:Data:Movies' does not exist in appsettings.json");

            var moviesFromUrl = await _externalApiService.GetMovies(moviesApiUrl);
            var res = moviesFromUrl
                .Select(movie => _mapper.Map<MovieResponse>(movie))
                .ToList();
            return res is not null ? Ok(res) : Problem(statusCode: 502);
        }
    }
}
