using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SPSUL.Models
{
    /// <summary>
    /// Služba pro správu oprávnìní uèitelù na základì jejich rolí.
    ///
    /// Jak to funguje:
    ///   1. Pøi každém requestu (pøes LoginRequiredAttribute) se naètou role uèitele z DB.
    ///   2. Role jsou mapovány na sadu oprávnìní (AppPermissions) pomocí statického slovníku.
    ///   3. Výsledná oprávnìní jsou UNION všech rolí (pokud má uèitel víc rolí).
    ///   4. Oprávnìní jsou cachována v IMemoryCache na 10 minut.
    ///
    /// Použití v controlleru:
    ///   bool muze = await _authService.HasPermissionAsync(AppPermissions.CrudTests);
    ///
    /// Použití v Razor view (pøes HttpContext.Items, pøednaèteno LoginRequired):
    ///   @if (Context.HasPermission(AppPermissions.CrudTests)) { ... }
    /// </summary>
    public class AuthorizationService
    {
        private readonly SpsulContext _ctx;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

        // Role name ? granted permissions
        private static readonly Dictionary<string, HashSet<string>> RolePermissionMap = new()
        {
            ["Administrátor"] = [
                AppPermissions.All,
                AppPermissions.ManageTests,
                AppPermissions.ManageStudents,
                AppPermissions.ManageClasses,
                AppPermissions.ManageQuestions,
                AppPermissions.CrudTests,
                AppPermissions.CrudTeachers,
                AppPermissions.CrudStudents,
                AppPermissions.ViewOnly
            ],
            ["Tvùrce"] = [
                AppPermissions.ManageTests,
                AppPermissions.ManageStudents,
                AppPermissions.ManageClasses,
                AppPermissions.ManageQuestions,
                AppPermissions.CrudTests,
                AppPermissions.CrudStudents,
                AppPermissions.ViewOnly
            ],
            ["Testátor"] = [
                AppPermissions.CrudTests,
                AppPermissions.ViewOnly
            ],
            ["Uèitelátor"] = [
                AppPermissions.CrudTeachers,
                AppPermissions.ViewOnly
            ],
            ["Studentátor"] = [
                AppPermissions.CrudStudents,
                AppPermissions.ViewOnly
            ],
            ["Hlediè"] = [
                AppPermissions.ViewOnly
            ]
        };

        public AuthorizationService(SpsulContext ctx, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _ctx = ctx;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets all role names for the current teacher (cached).
        /// </summary>
        public async Task<List<string>> GetRolesAsync()
        {
            var teacherId = GetTeacherId();
            if (teacherId == null) return [];

            var cacheKey = $"auth:roles:{teacherId}";
            if (_cache.TryGetValue(cacheKey, out List<string>? cached) && cached != null)
                return cached;

            var roles = await _ctx.TeacherRoles
                .AsNoTracking()
                .Where(tr => tr.TeacherId == teacherId.Value)
                .Include(tr => tr.Role)
                .Where(tr => tr.Role.IsActive)
                .Select(tr => tr.Role.Name)
                .ToListAsync();

            _cache.Set(cacheKey, roles, CacheExpiration);
            return roles;
        }

        /// <summary>
        /// Gets all effective permissions for the current teacher (union of all role permissions).
        /// </summary>
        public async Task<HashSet<string>> GetPermissionsAsync()
        {
            var roles = await GetRolesAsync();
            var permissions = new HashSet<string>();

            foreach (var role in roles)
            {
                if (RolePermissionMap.TryGetValue(role, out var perms))
                    permissions.UnionWith(perms);
            }

            // No roles = anonymous = Hlediè
            if (roles.Count == 0)
                permissions.Add(AppPermissions.ViewOnly);

            return permissions;
        }

        /// <summary>
        /// Checks if the current teacher has a specific permission.
        /// </summary>
        public async Task<bool> HasPermissionAsync(string permission)
        {
            var permissions = await GetPermissionsAsync();
            return permissions.Contains(permission);
        }

        /// <summary>
        /// Checks if the current teacher has ANY of the specified permissions.
        /// </summary>
        public async Task<bool> HasAnyPermissionAsync(params string[] permissions)
        {
            var effective = await GetPermissionsAsync();
            return permissions.Any(p => effective.Contains(p));
        }

        /// <summary>
        /// Checks if the current teacher is an administrator.
        /// </summary>
        public async Task<bool> IsAdminAsync()
        {
            return await HasPermissionAsync(AppPermissions.All);
        }

        /// <summary>
        /// Invalidates the cached roles for a specific teacher.
        /// Call after role changes.
        /// </summary>
        public void InvalidateCache(int teacherId)
        {
            _cache.Remove($"auth:roles:{teacherId}");
        }

        private int? GetTeacherId()
        {
            return _httpContextAccessor.HttpContext?.Session.GetInt32("TeacherId");
        }
    }
}
