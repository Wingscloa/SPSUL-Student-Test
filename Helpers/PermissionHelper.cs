using SPSUL.Models;

namespace SPSUL.Helpers
{
    /// <summary>
    /// Extension metody pro snadné ovìøování oprávnìní pøímo v Razor views.
    ///
    /// Jak funguje:
    ///   LoginRequiredAttribute pøi každém requestu pøednaète oprávnìní uèitele a uloží je
    ///   do HttpContext.Items["Permissions"] jako HashSet&lt;string&gt;.
    ///   Tyto extension metody pak z Items rychle pøeètou, zda dané oprávnìní existuje.
    ///
    /// Použití v Razor view:
    ///   @if (Context.HasPermission(AppPermissions.CrudTests))
    ///   {
    ///       &lt;button&gt;Vytvoøit test&lt;/button&gt;
    ///   }
    /// </summary>
    public static class PermissionHelper
    {
        /// <summary>
        /// Checks if the current user has a specific permission.
        /// </summary>
        public static bool HasPermission(this HttpContext context, string permission)
        {
            if (context.Items["Permissions"] is HashSet<string> perms)
                return perms.Contains(permission);
            return false;
        }

        /// <summary>
        /// Checks if the current user has ANY of the specified permissions.
        /// </summary>
        public static bool HasAnyPermission(this HttpContext context, params string[] permissions)
        {
            if (context.Items["Permissions"] is HashSet<string> perms)
                return permissions.Any(p => perms.Contains(p));
            return false;
        }
    }
}
