using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CardStore.DTOs;

namespace CardStore.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (UserDto?)context.HttpContext.Items["User"];
        if (user == null)
        {
            // Not logged in
            context.Result = new JsonResult(new { message = "Unauthorized" }) 
            { 
                StatusCode = StatusCodes.Status401Unauthorized 
            };
        }
    }
}