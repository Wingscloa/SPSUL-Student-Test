using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SPSUL.Models;

/// <summary>
/// Action filter, který ověří, zda má přihlášený učitel potřebné oprávnění.
///
/// Co dělá:
///   Pokud učitel nemá žádné z uvedených oprávnění:
///   - API endpointy (JSON) vrátí 403 s JSON odpovědí.
///   - Stránky (GET) přesměrují na /Error/403 (Forbidden view).
///
/// Použití:
///   [RequirePermission(AppPermissions.CrudTests)]              - vyžaduje jedno oprávnění
///   [RequirePermission(AppPermissions.CrudTests, AppPermissions.All)] - stačí jedno z nich
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAsyncActionFilter
{
    private readonly string[] _permissions;

    /// <summary>
    /// Requires the teacher to have ANY of the specified permissions.
    /// </summary>
    public RequirePermissionAttribute(params string[] permissions)
    {
        _permissions = permissions;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var authService = context.HttpContext.RequestServices.GetRequiredService<AuthorizationService>();
        var hasPermission = await authService.HasAnyPermissionAsync(_permissions);

        if (!hasPermission)
        {
            // API calls get JSON 403, pages get redirected to error page
            if (context.HttpContext.Request.Headers.Accept.ToString().Contains("application/json")
                || context.HttpContext.Request.Method != "GET")
            {
                context.Result = new JsonResult(new { message = "Nemáte oprávnění k této akci." })
                {
                    StatusCode = 403
                };
            }
            else
            {
                context.Result = new RedirectToActionResult("HttpStatusCodeHandler", "Error", new { statusCode = 403 });
            }
            return;
        }

        await next();
    }
}
