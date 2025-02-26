using Scalar.AspNetCore;
using Serilog;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Movies.Services.Services.Interfaces;
using Movies.Services.Profiles;
using Movies.Services.Services;
using Movies.Api.Middleware;

namespace Movies.Api
{
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
            // Dependency injection
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IExternalApiService, ExternalApiService>();
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddAutoMapper(typeof(MovieProfile));
            //builder.Services.AddAutoMapper(typeof(Program));

            // Add CORS configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // Get the token from the cookie instead of Authorization header
                            if (context.Request.Cookies.TryGetValue("jwt", out var token))
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
                builder.Configuration.GetSection("JwtSettings")["Audience1"]!,
                builder.Configuration.GetSection("JwtSettings")["Audience2"]!,
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
                app.UseSwagger(options =>
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
}
