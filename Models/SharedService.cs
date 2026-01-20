 using SPSUL.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SPSUL.Models
{
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
        public async Task<string?> GetNameAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return null;
            int? teacherId = session.GetInt32("TeacherId");
            if (teacherId == null) return null;

            string? name = await _cacheService.GetCacheValue(CacheKeys.TeacherName);

            if (string.IsNullOrEmpty(name))
            {
                Teacher? teacher = await _ctx.Teachers.Where(e => e.TeacherId == teacherId)
                    .Include(e => e.Titles).ThenInclude(e => e.Title)
                    .FirstOrDefaultAsync();

                if (teacher == null) return null;

                string titlePrefix = string.Join(' ', teacher.Titles.Select(e => e.Title.Shortcut));
                name = string.IsNullOrWhiteSpace(titlePrefix) ? $"{teacher.FirstName} {teacher.LastName}" : $"{titlePrefix} {teacher.FirstName} {teacher.LastName}";
                await _cacheService.SetCacheAsync(CacheKeys.TeacherName, name, TimeSpan.FromDays(7));
            }

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
