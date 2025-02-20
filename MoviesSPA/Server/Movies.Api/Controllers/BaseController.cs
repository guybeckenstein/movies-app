using Microsoft.AspNetCore.Mvc;

namespace Movies.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly IConfiguration _configuration;
        protected BaseController(IConfiguration configuration) => _configuration = configuration;
    }
}
