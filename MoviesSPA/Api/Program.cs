using Api.Middleware;
using Scalar.AspNetCore;
using Serilog;
using Services.Services.Interfaces;
using Services.Services;
using System.Text;
using Api.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();
// Log file configuration
builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
// Serilog is logging provider
builder.Logging.AddSerilog(Log.Logger);
// Dependency injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExternalApiService, ExternalApiService>();
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(typeof(Program));
// Registering action filters
builder.Services.AddScoped<ValidationFilterAttribute>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
                                                .WithOrigins("http://localhost:4200")
                                                .AllowAnyMethod()
                                                .AllowAnyHeader()
                                                .AllowCredentials());
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings")["Token"] ?? throw new ArgumentNullException("JWT Secret key is missing"))),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration.GetSection("JwtSettings")["Issuer"],
            ValidAudience = builder.Configuration.GetSection("JwtSettings")["Audience"],
            ValidateLifetime = true,
        };
    });

builder.Services.AddAuthorization();

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

var app = builder.Build();
app.UseCors("CorsPolicy");
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
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
