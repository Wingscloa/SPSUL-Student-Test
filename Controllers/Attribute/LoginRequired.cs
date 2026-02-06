using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class LoginRequiredAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Skip login check if AllowAnonymousTest attribute is present
        var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(m => m is AllowAnonymousTestAttribute);

        if (hasAllowAnonymous)
        {
            base.OnActionExecuting(context);
            return;
        }

        var userId = context.HttpContext.Session.GetInt32("TeacherId");

        if (userId == null)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
        }
        else
        {
            // Uložení userId do HttpContext.Items pro pozdější použití
            context.HttpContext.Items["CurrentUserId"] = userId.Value;
        }

        base.OnActionExecuting(context);
    }
}