 using SPSUL.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace SPSUL.Models
{
    /// <summary>
    /// Sdílená služba pro získávání informací o aktuálně přihlášeném učiteli.
    ///
    /// Proč existuje:
    ///   Napříč celou aplikací (controllery, ViewComponents, layout) se opakovaně
    ///   potřebuje jméno nebo ID přihlášeného učitele. Tato třída centralizuje tuto logiku
    ///   a výsledky cachuje, aby se databáze nezatěžovala zbytečnými dotazy.
    ///
    /// Cache strategie:
    ///   - GetTeacherAsync() cachuje entitu učitele v HttpContext.Items (platí jen pro daný request).
    ///   - GetNameAsync() navíc cachuje formátované jméno v IMemoryCache na 7 dní.
    /// </summary>
    public class SharedService
    {
        private readonly SpsulContext _ctx;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CacheService _cacheService;
        public SharedService(SpsulContext ctx, IHttpContextAccessor httpContextAccessor, CacheService cacheService) {
            _ctx = ctx;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Gets the current teacher entity (cached for the request via HttpContext.Items).
        /// </summary>
        public async Task<Teacher?> GetTeacherAsync()
        {
            const string itemKey = "CachedTeacher";
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            if (httpContext.Items.TryGetValue(itemKey, out var cached) && cached is Teacher t)
                return t;

            int? teacherId = GetTeacherId();
            if (teacherId == null) return null;

            var teacher = await _ctx.Teachers
                .AsNoTracking()
                .Where(e => e.TeacherId == teacherId)
                .Include(e => e.Titles).ThenInclude(e => e.Title)
                .FirstOrDefaultAsync();

            if (teacher != null)
                httpContext.Items[itemKey] = teacher;

            return teacher;
        }

        public async Task<string?> GetNameAsync()
        {
            string? name = _cacheService.Get("TeacherName");
            if (!string.IsNullOrEmpty(name))
                return name;

            Teacher? teacher = await GetTeacherAsync();
            if (teacher == null) return null;

            string titlePrefix = string.Join(' ', teacher.Titles.Select(e => e.Title.Shortcut));
            name = string.IsNullOrWhiteSpace(titlePrefix) ? $"{teacher.FirstName} {teacher.LastName}" : $"{titlePrefix} {teacher.FirstName} {teacher.LastName}";
            _cacheService.Set("TeacherName", name, TimeSpan.FromDays(7));

            return name;
        }

        public int? GetTeacherId()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return null;
            int? teacherId = session.GetInt32("TeacherId");
            return teacherId.HasValue ? teacherId : null;
        }
    }
}
