using Microsoft.Extensions.Logging;

namespace Movies.Services.Services;

public abstract class BaseService<T>(ILogger<T> logger)
{
    protected readonly ILogger<T> _logger = logger;
}
