using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

/// <summary>
/// Jednoduchý ochranný filter proti brute-force útokùm (pøíliš mnoho pokusù za krátkou dobu).
///
/// Jak funguje:
///   Poèítá poèet requiestù z konkrétní IP adresy pro danou akci.
///   Pokud poèet pøekroèí MaxAttempts bìhem WindowSeconds sekund, další request je zablokovaný.
///   Èítadá je uloženo v IMemoryCache.
///
/// Použití:
///   [RateLimit(MaxAttempts = 5, WindowSeconds = 300)]
///   public async Task&lt;IActionResult&gt; Login(LoginViewModel model) { ... }
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RateLimitAttribute : Attribute, IAsyncActionFilter
{
    public int MaxAttempts { get; set; } = 5;
    public int WindowSeconds { get; set; } = 300;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var cacheKey = $"ratelimit:{context.ActionDescriptor.DisplayName}:{ip}";

        var attempts = cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(WindowSeconds);
            return 0;
        });

        if (attempts >= MaxAttempts)
        {
            if (context.HttpContext.Request.Headers.Accept.ToString().Contains("application/json"))
            {
                context.Result = new JsonResult(new { message = "Pøíliš mnoho pokusù. Zkuste to znovu za nìkolik minut." })
                {
                    StatusCode = 429
                };
            }
            else
            {
                context.HttpContext.Items["RateLimited"] = true;
                var controller = context.Controller as Controller;
                controller?.TempData["TestError"] = "Pøíliš mnoho pokusù o pøihlášení. Zkuste to za 5 minut.";
                context.Result = new ViewResult { ViewName = "Login" };
            }
            return;
        }

        cache.Set(cacheKey, attempts + 1, TimeSpan.FromSeconds(WindowSeconds));
        await next();
    }
}
