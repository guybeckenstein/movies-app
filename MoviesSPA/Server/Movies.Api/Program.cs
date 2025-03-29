using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Filters.ActionFilters;
using Movies.Api.Middleware;
using Movies.Data.Repositories;
using Movies.Data.Repositories.Interfaces;
using Movies.Repository;
using Movies.Services.Profiles;
using Movies.Services.Services;
using Movies.Services.Services.Interfaces;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

namespace Movies.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSwaggerGen();
        // Log file configuration
        Log.Logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration)
            .Enrich
            .FromLogContext()
            .CreateLogger();
        // Serilog is logging provider
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger);
        // DB
        builder.Services
            .AddEntityFrameworkNpgsql()
            .AddDbContext<MyDbContext>(options => 
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        // Register dependency injections and classes
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IExternalApiService, ExternalApiService>();

        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddScoped<JwtAuthFilter>();
        // Add services to the container
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddAutoMapper([typeof(MovieProfile)]);
        //builder.Services.AddAutoMapper(typeof(Program));

        // Add CORS configuration
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "AllowLocalhost", configurePolicy: policy =>
            {
                policy.WithOrigins(["http://localhost:4200"])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(configureOptions: options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Get the token from the cookie instead of Authorization header
                        if (context.Request.Cookies.TryGetValue(builder.Configuration.GetSection("JwtSettings")["CookieName"] ?? throw new ArgumentNullException("JWT cookie name is missing"), out var token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings")["Token"] ?? throw new ArgumentNullException("JWT Secret key is missing"))),
                    ValidIssuer = builder.Configuration.GetSection("JwtSettings")["Issuer"],
                    ValidAudiences = new List<string>
                    {
                        builder.Configuration.GetSection("JwtSettings")["Audience1"] ?? throw new ArgumentNullException("JWT audience 1 is missing"),
                        builder.Configuration.GetSection("JwtSettings")["Audience2"] ?? throw new ArgumentNullException("JWT audience 2 is missing"),
                    },
                };
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();
        // Registering middleware
        app.UseMiddleware<LoggingMiddleware>();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(setupAction: options =>
            {
                options.RouteTemplate = "/openapi/{documentName}.json";
            });
            app.UseSwaggerUI();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowLocalhost");
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
