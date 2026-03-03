using Microsoft.Extensions.Caching.Memory;

namespace SPSUL.Models
{
    /// <summary>
    /// Pomocná služba pro ukládání dat do in-memory cache, vázaná na aktuálního učitele.
    /// 
    /// Problém, který řeší:
    ///   Při každém requestu by se jinak muselo znovu ptát do databáze na např. jméno učitele.
    ///   Tato třída ukládá výsledky do IMemoryCache pod klíčem "teacherId:klic",
    ///   takže každý učitel má svůj vlastní cache prostor.
    /// 
    /// Použití:
    ///   _cacheService.Set("TeacherName", "Jan Novák", TimeSpan.FromDays(7));
    ///   string? name = _cacheService.Get("TeacherName");
    /// </summary>
    public class CacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CacheService(IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Sets a cache value scoped to the current teacher.
        /// </summary>
        public void Set(string key, string value, TimeSpan expiration)
        {
            var fullKey = BuildKey(key);
            if (fullKey != null)
                _cache.Set(fullKey, value, expiration);
        }

        /// <summary>
        /// Gets a cache value scoped to the current teacher.
        /// </summary>
        public string? Get(string key)
        {
            var fullKey = BuildKey(key);
            if (fullKey == null) return null;

            _cache.TryGetValue(fullKey, out string? value);
            return value;
        }

        /// <summary>
        /// Removes a cache entry scoped to the current teacher.
        /// </summary>
        public void Remove(string key)
        {
            var fullKey = BuildKey(key);
            if (fullKey != null)
                _cache.Remove(fullKey);
        }

        private string? BuildKey(string key)
        {
            var teacherId = _httpContextAccessor.HttpContext?.Session.GetInt32("TeacherId");
            return teacherId.HasValue ? $"teacher:{teacherId}:{key}" : null;
        }
    }
}
