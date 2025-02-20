using Microsoft.Extensions.Logging;

namespace Movies.Services.Services
{
    public class BaseService<T>
    {
        protected readonly ILogger<T> _logger;
        protected BaseService(ILogger<T> logger) => _logger = logger;
    }
}
