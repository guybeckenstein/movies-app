using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.ActionFilters
{
    public class ValidationFilterAttribute: IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // We skip this when making requests from `localhost:4200` so it won't break the code
            if (!context.HttpContext.Request.Headers["Origin"].Equals("http://localhost:4200")) // TODO: should use `appsettings.json`
            {
                // Code we can reach when making requests from `localhost:7221`
                // Retrieve JWT token from cookies
                if (context.HttpContext.Request.Cookies.TryGetValue("jwt", out var jwt) && !string.IsNullOrEmpty(jwt))
                {
                    context.HttpContext.Items["token"] = jwt;
                }
                else
                {
                    context.Result = new BadRequestObjectResult("JWT token is missing in cookies.");
                    return;
                }
            }
            // Validate ModelState
            if (context.ModelState.IsValid is false)
            {
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
