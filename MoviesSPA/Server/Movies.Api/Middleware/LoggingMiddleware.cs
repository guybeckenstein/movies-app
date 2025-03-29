using System.Diagnostics;

namespace Movies.Api.Middleware;

public sealed class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<LoggingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        var stopwatch = Stopwatch.StartNew();

        var method = context.Request.Method;
        var path = context.Request.Path.ToString();

        _logger.LogInformation($"{method} {path} - Starting request");

        try
        {
            await _next(context); // Call the next middleware in the pipeline
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{method} {path} - Exception in request: {ex}{Environment.NewLine}" +
                $"{ex.StackTrace}");
            context.Response.StatusCode = 500; // Internal Server Error
            await context.Response.WriteAsync("An error occurred while processing the request.");
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogDebug($"{method} {path} - Completed request in {stopwatch.ElapsedMilliseconds.ToString()}ms");
            _logger.LogInformation($"{method} {path} - Completed request");
        }
    }
}
