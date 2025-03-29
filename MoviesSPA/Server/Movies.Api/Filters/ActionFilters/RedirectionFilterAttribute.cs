using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Movies.Api.Filters.ActionFilters;

public class RedirectionFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Checks if there is already a JWT token cookie
        if (context.HttpContext.Request.Cookies.TryGetValue("jwt", out var _))
        {
            context.Result = new RedirectResult("/api/Movies/");
            return;
        }
    }
}
