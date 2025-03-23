using AutoMapper;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Movies.Models.DTOs;
using Movies.Models.Models;
using Movies.Services.Profiles;
using Movies.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.Services.Services;

public sealed class ExternalApiService : BaseService<IExternalApiService>, IExternalApiService
{
    private readonly IMapper _mapper;
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

    public async Task<IEnumerable<MovieResponse>> GetMovies(string url)
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
                .Select(_mapper.Map<MovieResponse>)
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
