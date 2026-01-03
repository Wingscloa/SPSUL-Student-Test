using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class LoginRequiredAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = context.HttpContext.Session.GetInt32("TeacherId");

        if (userId == null)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
        }

        base.OnActionExecuting(context);
    }
}