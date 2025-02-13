using Api.Middleware;
using Scalar.AspNetCore;
using Serilog;
using Services.Services.Interfaces;
using Services.Services;
using Api.Profiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        // Log file configuration
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        // Serilog is logging provider
        builder.Logging.AddSerilog(Log.Logger);
        // Dependency injection
        builder.Services.AddScoped<IExternalApiService, ExternalApiService>();
        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAutoMapper(typeof(MovieProfile));

        var app = builder.Build();
        // Registering middleware
        app.UseMiddleware<LoggingMiddleware>();
        // Dependency injection
        // TODO: add dependency injection
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "/openapi/{documentName}.json";
            });
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        // Add Authentication service
                        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(options =>
                            {
                                options.RequireHttpsMetadata = false;
                                options.Authority = "https://your-auth-server.com";  // JWT issuer URL (e.g., IdentityServer or Auth0)
                                options.Audience = "your-api-name";  // JWT Audience (your API name)
                                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    ValidIssuer = "https://your-auth-server.com",
                                    ValidAudience = "your-api-name",
                                    ClockSkew = TimeSpan.Zero  // Optional: Set Clock Skew for token expiry tolerance
                                };
                            });

                        // Add controllers and other services
                        services.AddControllers();

                        // Add AutoMapper and any other required services
                        services.AddAutoMapper(typeof(MovieProfile));

                    })
                    .Configure(app =>
                    {
                        app.UseAuthentication(); // Add Authentication middleware
                        app.UseAuthorization();  // Add Authorization middleware

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                });

    public void Configure(IApplicationBuilder app)
    {
        app.UseAuthentication();  // Must be before UseAuthorization
        app.UseAuthorization();
    }
}
