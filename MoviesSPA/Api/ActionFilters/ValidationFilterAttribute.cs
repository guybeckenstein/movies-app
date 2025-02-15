using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.ActionFilters
{
    public class ValidationFilterAttribute: IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
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
            // Validate ModelState
            if (context.ModelState.IsValid is false)
            {
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
