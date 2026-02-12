using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using SPSUL.Models.Data;

namespace SPSUL.Models
{
    /// <summary>
    /// Singleton cache for semi-static lookup data (classes, fields, question types).
    /// Data is cached for 5 minutes and invalidated on write via Invalidate().
    /// </summary>
    public class LookupCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

        private const string KeyActiveClasses = "lookup:classes:active";
        private const string KeyAllClasses = "lookup:classes:all";
        private const string KeyActiveFields = "lookup:fields:active";
        private const string KeyAllFields = "lookup:fields:all";
        private const string KeyActiveTypes = "lookup:types:active";
        private const string KeyRoles = "lookup:roles";
        private const string KeyTitles = "lookup:titles";

        public LookupCacheService(IMemoryCache cache, IServiceScopeFactory scopeFactory)
        {
            _cache = cache;
            _scopeFactory = scopeFactory;
        }

        public async Task<List<Classes>> GetActiveClassesAsync()
        {
            return await GetOrCreateAsync(KeyActiveClasses, async ctx =>
                await ctx.Classes.AsNoTracking()
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync());
        }

        public async Task<List<Classes>> GetAllClassesAsync()
        {
            return await GetOrCreateAsync(KeyAllClasses, async ctx =>
                await ctx.Classes.AsNoTracking()
                    .Include(c => c.ClassesStudents)
                    .ToListAsync());
        }

        public async Task<List<StudentField>> GetActiveFieldsAsync()
        {
            return await GetOrCreateAsync(KeyActiveFields, async ctx =>
                await ctx.StudentFields.AsNoTracking()
                    .Where(f => f.IsActive)
                    .OrderBy(f => f.Name)
                    .ToListAsync());
        }

        public async Task<List<StudentField>> GetAllFieldsAsync()
        {
            return await GetOrCreateAsync(KeyAllFields, async ctx =>
                await ctx.StudentFields.AsNoTracking()
                    .ToListAsync());
        }

        public async Task<List<QuestionType>> GetActiveTypesAsync()
        {
            return await GetOrCreateAsync(KeyActiveTypes, async ctx =>
                await ctx.QuestionTypes.AsNoTracking()
                    .Where(t => t.IsActive)
                    .ToListAsync());
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await GetOrCreateAsync(KeyRoles, async ctx =>
                await ctx.Roles.AsNoTracking().ToListAsync());
        }

        public async Task<List<Title>> GetTitlesAsync()
        {
            return await GetOrCreateAsync(KeyTitles, async ctx =>
                await ctx.Titles.AsNoTracking().ToListAsync());
        }

        /// <summary>
        /// Call after any write operation that changes lookup data (classes, fields, types, etc.).
        /// </summary>
        public void InvalidateAll()
        {
            _cache.Remove(KeyActiveClasses);
            _cache.Remove(KeyAllClasses);
            _cache.Remove(KeyActiveFields);
            _cache.Remove(KeyAllFields);
            _cache.Remove(KeyActiveTypes);
            _cache.Remove(KeyRoles);
            _cache.Remove(KeyTitles);
        }

        public void InvalidateClasses()
        {
            _cache.Remove(KeyActiveClasses);
            _cache.Remove(KeyAllClasses);
        }

        public void InvalidateFields()
        {
            _cache.Remove(KeyActiveFields);
            _cache.Remove(KeyAllFields);
        }

        private async Task<List<T>> GetOrCreateAsync<T>(string key, Func<SpsulContext, Task<List<T>>> factory)
        {
            if (_cache.TryGetValue(key, out List<T>? cached) && cached != null)
                return cached;

            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<SpsulContext>();
            var data = await factory(ctx);

            _cache.Set(key, data, DefaultExpiration);
            return data;
        }
    }
}
