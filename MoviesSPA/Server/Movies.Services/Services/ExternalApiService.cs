using Flurl.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using AutoMapper;
using System.Linq;
using Movies.Services.Profiles;
using Movies.Services.Services.Interfaces;
using Movies.Models.Models;
using Movies.Models.DTOs;

namespace Movies.Services.Services
{
    public class ExternalApiService : BaseService<IExternalApiService>, IExternalApiService
    {
        private readonly IMapper _mapper; // Converts `MovieRequest` to `MovieResponse`, but unused because AutoMapper broke the code
        private readonly MapperConfiguration _mapperConfiguration;
        public ExternalApiService(ILogger<IExternalApiService> logger) : base(logger)
        {
            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MovieProfile>();
            });
            _mapperConfiguration.AssertConfigurationIsValid();
            _mapper = _mapperConfiguration.CreateMapper();
        }

        public async Task<List<MovieResponse>> GetMovies(string url)
        {
            _logger.LogInformation($"ExternalApiService.GetMovies - Starting request{Environment.NewLine}" +
                $"{url}");
            // HTTP request using flurl as HTTP client
            var movies = await $"{url}"
                .GetJsonAsync<List<MovieRequest>>();
            var res = new List<MovieResponse>();

            if (movies.Count == 0)
            {
                _logger.LogWarning($"ExternalApiService.GetMovies - No data returned from{Environment.NewLine}" +
                    $"{url}");
            }
            else
            {
                // Instead of a List<MovieRequest>, returning List<MovieResponse>
                res = movies
                    .Select(movie => _mapper.Map<MovieResponse>(movie))
                    .ToList();

                if (res is null)
                {
                    throw new AutoMapperMappingException("ExternalApiService.GetMovies - `MovieProfile` failed to map from `MovieRequest` to `MovieResponse`");
                }
                _logger.LogInformation($"ExternalApiService.GetMovies - Finished request{Environment.NewLine}" +
                    $"{url}");
            }

            return res;
        }
    }
}
