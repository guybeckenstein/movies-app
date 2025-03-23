using Microsoft.AspNetCore.Mvc;

namespace Movies.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController(IConfiguration configuration) : ControllerBase
{
    protected readonly IConfiguration _configuration = configuration;
}
