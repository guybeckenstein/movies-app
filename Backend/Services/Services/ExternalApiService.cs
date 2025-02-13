using Flurl.Http;
using Microsoft.Extensions.Logging;
using Models.Models;
using Services.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Services.Services
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly ILogger<IExternalApiService> _logger;
        public ExternalApiService(ILogger<IExternalApiService> logger)
        {
            _logger = logger;
        }

        public async Task<List<MovieRequest>> GetMovies(string url)
        {
            _logger.LogInformation($"ExternalApiService.GetMovies - Starting request{Environment.NewLine}" +
                $"{url}");
            var movies = await $"{url}"
                .GetJsonAsync<List<MovieRequest>>();
            if (movies.Count == 0)
            {
                _logger.LogWarning($"ExternalApiService.GetMovies - No data returned from{Environment.NewLine}" +
                    $"{url}");
            }
            _logger.LogInformation($"ExternalApiService.GetMovies - Done request{Environment.NewLine}" +
                $"{url}");
            return movies;
        }
    }
}
