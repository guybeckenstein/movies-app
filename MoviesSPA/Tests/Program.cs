using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Services;
using Services.Services.Interfaces;

namespace Test
{
    class Program
    {
        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddScoped<IAuthService, AuthService>();
                    services.AddScoped<IExternalApiService, ExternalApiService>();
                });
    }
}
