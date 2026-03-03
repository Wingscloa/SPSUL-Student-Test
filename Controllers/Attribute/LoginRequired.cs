using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SPSUL.Models;

/// <summary>
/// Action filter, který chrání controllery a akce vyžadující přihlášení.
///
/// Co dělá:
///   1. Zkontroluje, zda je učitel přihlášen (session obsahuje TeacherId).
///   2. Pokud ne, přesměruje na Login stránku.
///   3. Pokud ano, uloží ID do HttpContext.Items["CurrentUserId"] pro pozdější použití.
///   4. Přednačte oprávnění učitele do HttpContext.Items["Permissions"] pro Razor views.
///
/// Použití:
///   [LoginRequired]           - na třídě controlleru (chrání všechny akce)
///   [AllowAnonymousTest]      - na konkrétní akci přeskočí tuto kontrolu
/// </summary>

public class LoginRequiredAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Skip login check if AllowAnonymousTest attribute is present
        var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(m => m is AllowAnonymousTestAttribute);

        if (hasAllowAnonymous)
        {
            await next();
            return;
        }

        var userId = context.HttpContext.Session.GetInt32("TeacherId");

        if (userId == null)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        // Store userId for later use
        context.HttpContext.Items["CurrentUserId"] = userId.Value;

        // Preload permissions into HttpContext.Items for views
        var authService = context.HttpContext.RequestServices.GetRequiredService<AuthorizationService>();
        var permissions = await authService.GetPermissionsAsync();
        context.HttpContext.Items["Permissions"] = permissions;

        await next();
    }
}