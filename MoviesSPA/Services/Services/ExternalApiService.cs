using Flurl.Http;
using Microsoft.Extensions.Logging;
using Models.Models;
using Services.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Models.DTOs;
using AutoMapper;
using System.Linq;

namespace Services.Services
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly ILogger<IExternalApiService> _logger;
        private readonly IMapper _mapper; // Converts `MovieRequest` to `MovieResponse`, but unused because AutoMapper broke the code
        public ExternalApiService(ILogger<IExternalApiService> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
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
                // Not using AutoMapper because it breaks the code
                // Instead of a List<MovieRequest>, returning List<MovieResponse>
                res = movies
                    .Select(movie =>
                    new MovieResponse
                    {
                        Title = movie.Title,
                        Year = movie.Year,
                        Genres = string.IsNullOrEmpty(movie.Genre)
                                ? new List<string>()
                                : movie.Genre.Split(new string[] { ", " }, StringSplitOptions.None).ToList(),
                        PosterImg = movie.Poster
                    })?
                    // .Select(movie => _mapper.Map<MovieResponse>(movie))?
                    .ToList();

                // if (res is null)
                // {
                //     throw new AutoMapperMappingException("ExternalApiService.GetMovies - `MovieProfile` failed to map from `MovieRequest` to `MovieResponse`");
                // }
                _logger.LogInformation($"ExternalApiService.GetMovies - Finished request{Environment.NewLine}" +
                    $"{url}");
            }

            return res;
        }
    }
}
